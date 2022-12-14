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

        static int kostil = 0;

        static ConsoleKeyInfo button;
        enum key { UP, DOWN, RIGHT, LEFT, FIRE, END, TRASH }
        static key pressed;

        static string[] bullet = new string[1] { "->" };

        static string[] playerModel = new string[File.ReadAllLines(path + @"\player.txt").GetLength(0)];

        static char[,] map = new char[mapSizeY, mapSizeX];
        static List<int[]> currentPlayerPos = new List<int[]> { };
        static List<int[]> currentPlayerBulletPos = new List<int[]> { };

        static void Setup()
        {
            Console.SetWindowSize(100, 40);
            Console.CursorVisible = false;

            playerModel = File.ReadAllLines(path + @"\player.txt");

            playerY = mapSizeY / 2;
            playerX = mapSizeX / 8;

            InsertModel(playerY, playerX, playerModel, currentPlayerPos);

            for (int i = 0; i < mapSizeX + 3; i++)
                Console.Write("@");

            Console.WriteLine();

            for (int i = 0; i < mapSizeY - 1; i++)
            {
                Console.Write("@");

                for (int j = 0; j < mapSizeX + 1; j++)
                {
                    Console.Write(" ");
                }

                Console.WriteLine("@");
            }

            for (int i = 0; i < mapSizeX + 3; i++)
                Console.Write("@");
        }
        static void Draw()
        {
            DrawModel(playerY, playerX, playerModel);
            for (int i = 0; i < currentPlayerBulletPos.Count; i += 2)
            {
                DrawModel(currentPlayerBulletPos[i][0], currentPlayerBulletPos[i][1], bullet);
            }

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
                    case ' ':
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
                    if(playerX + playerModel[0].Length < mapSizeX - 1)
                    InsertModel(playerY + playerModel.GetLength(0) - (playerModel.GetLength(0) - 1) / 2 - 1, playerX + playerModel[0].Length, bullet, currentPlayerBulletPos);
                    break;
                case key.END:
                    for (int i = 0; i < mapSizeY; i++)
                    {
                        for (int j = 0; j < mapSizeX; j++)
                        {
                            File.AppendAllText(path + @"\debug.txt", string.Concat(map[i, j]));
                        }
                        File.AppendAllText(path + @"\debug.txt", "\n");
                    }
                    break;
            }

            if (kostil >= 2)
            {
                kostil = 0;
                MoveBullet();
            }
        }
        static void Main()
        {
            Timer speedOfBullet = new Timer(Timer, null, 0, 50);
            Setup();
            while (true)
            {
                Logic();
                Draw();
                Input();
            }
        }
        static void MovePlayer(int modY, int modX)
        {
            if (playerY + modY + playerModel.GetLength(0) != mapSizeY && playerY + modY >= 0 && playerX + modX + playerModel[0].Length != mapSizeX && playerX + modX >= 0)
            {
                DeleteModel(playerY, playerX, playerModel, currentPlayerPos);
                InsertModel(playerY += modY, playerX += modX, playerModel, currentPlayerPos);
            }
        }
        static void InsertModel(int y, int x, string[] Model, List<int[]> currentPos)
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
                    Console.SetCursorPosition(x + j + 3, y + i + 1);
                    Console.Write("\b \b");
                }
            }
            if (Model.Equals(bullet))
            { 
                currentPos.RemoveAt(0);
                currentPos.RemoveAt(0);
            }
            else
                currentPos.Clear(); 
        }
        static void DrawModel(int y, int x, string[] Model)
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    Console.SetCursorPosition(x + j + 2, y + i + 1);
                    Console.Write(map[y + i, x + j]);
                }
            }
        }
        static void MoveBullet()
         {
            for(int i = 0; i < currentPlayerBulletPos.Count; i += 2)
            {
                int y = currentPlayerBulletPos[0][0];
                int x = currentPlayerBulletPos[0][1];
                DeleteModel(y, x, bullet, currentPlayerBulletPos);
                if (x < (mapSizeX - 3))
                {
                    InsertModel(y, x + 2, bullet, currentPlayerBulletPos);
                }
            }
        }
        static void Timer(object o)
        {
            kostil++;
        }
    }
}