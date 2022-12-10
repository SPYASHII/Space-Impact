using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace SpaceImpact
{
    class Game
    {
        static string path = @"G:\Programming\Space Impact\files";
        static int mapSizeY = 30, mapSizeX = 70;
        static int playerY, playerX;

        static ConsoleKeyInfo button;
        enum key { UP, DOWN, RIGHT, LEFT, FIRE, END, TRASH }
        static key pressed;

        static string[] playerModel = new string[File.ReadAllLines(path + @"\player.txt").GetLength(0)];

        static char[,] map = new char[mapSizeY, mapSizeX];
        static List<int []> currentPlayerPos = new List<int []> { };

        static void Setup()
        {
            Console.SetWindowSize(100,40);

            playerModel = File.ReadAllLines(path + @"\player.txt");

            playerY = mapSizeY / 2;
            playerX = mapSizeX / 8;

            InsertModel(playerY,playerX, playerModel, currentPlayerPos);

            for (int i = 0; i < mapSizeX + 2; i++)
                Console.Write("@");

            Console.WriteLine();

            for (int i = 0; i < mapSizeY; i++)
            {
                Console.Write("@");

                for (int j = 0; j < mapSizeX; j++)
                {
                    Console.Write(map[i,j]);
                }

                Console.WriteLine("@");
            }

            for (int i = 0; i < mapSizeX + 2; i++)
                Console.Write("@");
        }
        static void Draw()
        {
            Thread.Sleep(50);

        }
        static void Input()
        {
            pressed = key.TRASH;
            while (Console.KeyAvailable)
            {
                button = Console.ReadKey(true);

                    switch (button.KeyChar)
                    {
                        case 'A':
                        case 'a':
                            pressed = key.LEFT;
                            break;
                        case 'D':
                        case 'd':
                            pressed = key.RIGHT;
                            break;
                        case 'W':
                        case 'w':
                            pressed = key.UP;
                            break;
                        case 'S':
                        case 's':
                            pressed = key.DOWN;
                            break;
                        case 'H':
                        case 'h':
                            pressed = key.FIRE;
                            break;
                        case '\u001b':
                            pressed = key.END;
                            break;
                        default:
                            break;
                    }
            }
        }
        static void Logic()
        {
            switch (pressed)
            {
                case key.UP:
                    MovePlayer(-1, 0);
                    break;
                case key.DOWN:
                    MovePlayer(1, 0);
                    break;
                case key.LEFT:
                    MovePlayer(0, -1);
                    break;
                case key.RIGHT:
                    MovePlayer(0, 1);
                    break;
                case key.FIRE:
                    
                    break;
                //case key.END:
                //    end_game = false;
                //    break;
            }

        }
        static void Main()
        {
            Setup();
            while(true)
            {
                Logic();
                Draw();
                Input();
            }
        }
        static void MovePlayer(int modY, int modX)
        {
            DeleteModel(playerY, playerX, playerModel, currentPlayerPos);
            InsertModel(playerY += modY, playerX += modX, playerModel, currentPlayerPos);
        }
        static void InsertModel(int y, int x, string [] Model, List<int[]> currentPos)
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    map[y + i, x + j] = Model[i].ToCharArray()[j];
                    currentPos.Add(new int[2] { y + i, x + j });
                }
            }
        }
        static void DeleteModel(int y, int x, string[] Model, List<int[]> currentPos)
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    map[y + i, x + j] = ' ';
                }
            }
            currentPos.Clear();
        }
        static void DrawModel(int y, int x)
        {

        }
        static void DelModelFromScr(int y, int x)
        {

        }
    }
}