using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinalAssignment
{
    public class Character
    {
        public double Strength;

        public Character()
        {
            NeuralNetWork netWork = new NeuralNetWork(6, 6);

            var trainingInputs = NeuralNetWork.RandomSizeSixArray();
            var trainingOutputs = NeuralNetWork.MatrixTranspose(NeuralNetWork.RandomSizeSixArray());
            netWork.Train(trainingInputs, trainingOutputs, 10000);

            List<double> weights = RetrieveWeights(netWork.SynapsesMatrix);
            Strength = Math.Round(AddAllValues(weights), 3);
            if (Strength < 0) { Strength *= -1; }
        }

        double AddAllValues(List<double> values)
        {
            double sum = 0;
            foreach(double val in values)
            {
                sum += val;
            }
            return sum;
        }

        static List<double> RetrieveWeights(double[,] matrix)
        {
            List<double> weights = new List<double>();

            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    weights.Add(matrix[i, j]);
                }
            }

            return weights;
        }
    }

    class NeuralNetWork
    {
        private Random _radomObj;

        public NeuralNetWork(int synapseMatrixColumns, int synapseMatrixLines)
        {
            SynapseMatrixColumns = synapseMatrixColumns;
            SynapseMatrixLines = synapseMatrixLines;

            _Init();
        }

        public static double[,] RandomSizeSixArray()
        {
            Random rand = new Random();

            //This is one of the dumbest things I have ever written, so I am assuming this is very, very wrong:
            return new double[,] { { rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2) },
            { rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2) },
            { rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2) },
            { rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2) },
            { rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2) },
            { rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2), rand.Next(0, 2) } };
        }

        public int SynapseMatrixColumns { get; }
        public int SynapseMatrixLines { get; }
        public double[,] SynapsesMatrix { get; private set; }

        /// <summary>
        /// Initialize the ramdom object and the matrix of ramdon weights
        /// </summary>
        private void _Init()
        {
            // make sure that for every instance of the neural network we are geting the same radom values
            _radomObj = new Random(1);
            _GenerateSynapsesMatrix();
        }

        /// <summary>
        /// Generate our matrix with the weight of the synapses
        /// </summary>
        private void _GenerateSynapsesMatrix()
        {
            SynapsesMatrix = new double[SynapseMatrixLines, SynapseMatrixColumns];

            for (var i = 0; i < SynapseMatrixLines; i++)
            {
                for (var j = 0; j < SynapseMatrixColumns; j++)
                {
                    SynapsesMatrix[i, j] = (2 * _radomObj.NextDouble()) - 1;
                }
            }
        }

        /// <summary>
        /// Calculate the sigmoid of a value
        /// </summary>
        /// <returns></returns>
        private double[,] _CalculateSigmoid(double[,] matrix)
        {

            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    var value = matrix[i, j];
                    matrix[i, j] = 1 / (1 + Math.Exp(value * -1));
                }
            }
            return matrix;
        }

        /// <summary>
        /// Calculate the sigmoid derivative of a value
        /// </summary>
        /// <returns></returns>
        private double[,] _CalculateSigmoidDerivative(double[,] matrix)
        {
            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    var value = matrix[i, j];
                    matrix[i, j] = value * (1 - value);
                }
            }
            return matrix;
        }

        /// <summary>
        /// Will return the outputs give the set of the inputs
        /// </summary>
        public double[,] Think(double[,] inputMatrix)
        {
            var productOfTheInputsAndWeights = MatrixDotProduct(inputMatrix, SynapsesMatrix);

            return _CalculateSigmoid(productOfTheInputsAndWeights);

        }

        /// <summary>
        /// Train the neural network to achieve the output matrix values
        /// </summary>
        public void Train(double[,] trainInputMatrix, double[,] trainOutputMatrix, int interactions)
        {
            // we run all the interactions
            for (var i = 0; i < interactions; i++)
            {
                // calculate the output
                var output = Think(trainInputMatrix);

                // calculate the error
                var error = MatrixSubstract(trainOutputMatrix, output);
                var curSigmoidDerivative = _CalculateSigmoidDerivative(output);
                var error_SigmoidDerivative = MatrixProduct(error, curSigmoidDerivative);

                // calculate the adjustment :) 
                var adjustment = MatrixDotProduct(MatrixTranspose(trainInputMatrix), error_SigmoidDerivative);

                SynapsesMatrix = MatrixSum(SynapsesMatrix, adjustment);
            }
        }

        /// <summary>
        /// Transpose a matrix
        /// </summary>
        /// <returns></returns>
        public static double[,] MatrixTranspose(double[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Sum one matrix with another
        /// </summary>
        /// <returns></returns>
        public static double[,] MatrixSum(double[,] matrixa, double[,] matrixb)
        {
            var rowsA = matrixa.GetLength(0);
            var colsA = matrixa.GetLength(1);

            var result = new double[rowsA, colsA];

            for (int i = 0; i < rowsA; i++)
            {
                for (int u = 0; u < colsA; u++)
                {
                    result[i, u] = matrixa[i, u] + matrixb[i, u];
                }
            }

            return result;
        }

        /// <summary>
        /// Subtract one matrix from another
        /// </summary>
        /// <returns></returns>
        public static double[,] MatrixSubstract(double[,] matrixa, double[,] matrixb)
        {
            var rowsA = matrixa.GetLength(0);
            var colsA = matrixa.GetLength(1);

            var result = new double[rowsA, colsA];

            for (int i = 0; i < rowsA; i++)
            {
                for (int u = 0; u < colsA; u++)
                {
                    result[i, u] = matrixa[i, u] - matrixb[i, u];
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplication of a matrix
        /// </summary>
        /// <returns></returns>
        public static double[,] MatrixProduct(double[,] matrixa, double[,] matrixb)
        {
            var rowsA = matrixa.GetLength(0);
            var colsA = matrixa.GetLength(1);

            var result = new double[rowsA, colsA];

            for (int i = 0; i < rowsA; i++)
            {
                for (int u = 0; u < colsA; u++)
                {
                    result[i, u] = matrixa[i, u] * matrixb[i, u];
                }
            }

            return result;
        }

        /// <summary>
        /// Dot Multiplication of a matrix
        /// </summary>
        /// <returns></returns>
        public static double[,] MatrixDotProduct(double[,] matrixa, double[,] matrixb)
        {

            var rowsA = matrixa.GetLength(0);
            var colsA = matrixa.GetLength(1);

            var rowsB = matrixb.GetLength(0);
            var colsB = matrixb.GetLength(1);

            if (colsA != rowsB)
                throw new Exception("Matrices dimensions don't fit.");

            var result = new double[rowsA, colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    for (int k = 0; k < rowsB; k++)
                        result[i, j] += matrixa[i, k] * matrixb[k, j];
                }
            }
            return result;
        }

    }
}
