using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

public enum GameState { WANDERING, FIGHT_START, FIGHT_END, GAME_OVER }

namespace AIFinalAssignment
{
    public class Game
    {
        InputReader Reader;
        Character Player;
        Character CurrentEnemy;
        GameState State = GameState.WANDERING;
        ConsoleColor DefaultColor;

        int Wins;

        public Game(ConsoleColor defaultColor = ConsoleColor.White)
        {
            DefaultColor = defaultColor;

            StartGame();

            Reader = new InputReader();
            Reader.KeyPressed += Reader_KeyPressed;
            Reader.Activate();
        }

        public void StartGame()
        {
            Player = new Character();

            PrintMessage("You find yourself in a maze.", true);
            PrintMessage("Use WASD to explore your surroundings.", false, ConsoleColor.Cyan);

            Wins = 0;
            State = GameState.WANDERING;
        }

        private void Reader_KeyPressed(object? sender, EventArgs e)
        {
            switch(State)
            {
                case GameState.WANDERING:
                    Wander();
                    break;
                case GameState.FIGHT_START:
                    BeginFight();
                    break;
                case GameState.FIGHT_END:
                    EndFight();
                    break;
                case GameState.GAME_OVER:
                    DetermineContinue();
                    break;
            }
        }

        private void Wander()
        {
            switch(Reader.PressedKey)
            {
                case "W":
                    PrintMessage("You walk forward.", true);
                    break;
                case "A":
                    PrintMessage("You walk to the left.", true);
                    break;
                case "S":
                    PrintMessage("You walk backwards.", true);
                    break;
                case "D":
                    PrintMessage("You walk to the right.", true);
                    break;
                default:
                    PrintMessage("You don't walk at all.", true);
                    break;
            }

            Random rand = new Random();
            if (rand.Next(1, 4) != 2)
            {
                PrintMessage("Use WASD to explore your surroundings.", false, ConsoleColor.Cyan);
            }
            else
            {
                PrintMessage("A monster appears!", false, ConsoleColor.Red);
                State = GameState.FIGHT_START;
            }
        }

        private void BeginFight()
        {
            CurrentEnemy = new Character();
            PrintMessage($"You have a strength value of {Player.Strength}.", true, ConsoleColor.Cyan);
            PrintMessage($"The monster has a strength value of {CurrentEnemy.Strength}.", false, ConsoleColor.Red);
            State = GameState.FIGHT_END;
        }

        private void EndFight()
        {
            if (Player.Strength >= CurrentEnemy.Strength)
            {
                Wins++;
                PrintMessage($"You defeated the monster!", true, ConsoleColor.Cyan);
                PrintMessage($"You have now defeated {Wins} monster(s).", false);
                State = GameState.WANDERING;
            }
            else
            {
                PrintMessage($"The monster defeated you...", true, ConsoleColor.Red);
                PrintMessage("Would you like to try again? [Y/N]", false);
                State = GameState.GAME_OVER;
            }
        }

        public void DetermineContinue()
        {
            if (Reader.PressedKey == "Y")
            {
                StartGame();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void PrintMessage(string message, bool clear, ConsoleColor color = ConsoleColor.White)
        {
            if (clear)
            {
                Clear();
            }

            ForegroundColor = color;
            WriteLine(message);
            ForegroundColor = DefaultColor;
        }
    }
}
