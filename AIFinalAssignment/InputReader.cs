using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace AIFinalAssignment
{
    public class InputReader
    {
        public event EventHandler KeyPressed;
        public string PressedKey = "";
        private bool ReadingInputs = false;

        public InputReader() { }

        public void Activate() { ReadingInputs = true; ReadInputs(); }
        public void Deactivate() { ReadingInputs = false; }

        public void ReadInputs()
        {
            while (ReadingInputs)
            {
                PressedKey = ReadKey().KeyChar.ToString().ToUpper();
                KeyPressed.Invoke(this, new EventArgs());
            }
        }
    }
}
