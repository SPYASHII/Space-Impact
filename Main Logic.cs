using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Space_Impact
{
    public partial class Game
    {
        public static void Menu() //Меню 
        {
            Console.CursorVisible = false;

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.SetWindowSize(67, mapSizeY + 5);

            string[] spaceImpact = new string[File.ReadAllLines(path + @"\enemies\enemy0.txt").GetLength(0)];
            spaceImpact = File.ReadAllLines(path + @"\spaceimpact.txt");

            Console.SetCursorPosition(0, 3);

            foreach (var i in spaceImpact)
            {
                Console.WriteLine(i);
            }

            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, Console.WindowHeight / 2);
            Console.WriteLine("Press any key to Start");

            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 + 2);
            Console.WriteLine("Press ESC to Exit");

            button = Console.ReadKey(true);
            if (button.KeyChar == '\u001b')
                exit = true;

            Console.Clear();
        }
        public static void Setup() //Первоначальная настройка
        {
            gameover = false;

            currentPlayerPos.Clear();
            currentPlayerBulletPos.Clear();
            currentEnemyPos.Clear();
            currentEnemyBulletPos.Clear();

            map = new char[mapSizeY, mapSizeX];

            Console.SetWindowSize(mapSizeX + 3, mapSizeY + 5);

            playerModel = File.ReadAllLines(path + @"\player.txt");

            enemyModels.Add(new string[File.ReadAllLines(path + @"\enemies\enemy0.txt").GetLength(0)]);
            enemyModels[0] = File.ReadAllLines(path + @"\enemies\enemy0.txt");

            enemyModels.Add(new string[File.ReadAllLines(path + @"\enemies\enemy1.txt").GetLength(0)]);
            enemyModels[1] = File.ReadAllLines(path + @"\enemies\enemy1.txt");

            health = 3;
            score = 0;

            playerY = mapSizeY / 2;
            playerX = mapSizeX / 8;

            DrawMap();

            InsertModel(playerY, playerX, playerModel, currentPlayerPos);
            DrawModel(playerY, playerX, playerModel);

            DrawDelHealth();
            DrawScore();
        }

        public static void Input() //Отслеживание ввода 
        {
            pressed = Key.TRASH;
            while (Console.KeyAvailable)
            {
                button = Console.ReadKey(true);

                switch (button.KeyChar)
                {
                    case 'A':
                    case 'a':
                        pressed = Key.LEFT;
                        break;
                    case 'D':
                    case 'd':
                        pressed = Key.RIGHT;
                        break;
                    case 'W':
                    case 'w':
                        pressed = Key.UP;
                        break;
                    case 'S':
                    case 's':
                        pressed = Key.DOWN;
                        break;
                    case ' ':
                        pressed = Key.FIRE;
                        break;
                    case '\u001b':
                        pressed = Key.END;
                        break;
                    default:
                        break;
                }
            }
        }
        public static void Logic() //Логика игры
        {
            switch (pressed)
            {
                case Key.UP:
                    MovePlayer(-1, 0);
                    break;
                case Key.DOWN:
                    MovePlayer(1, 0);
                    break;
                case Key.LEFT:
                    MovePlayer(0, -1);
                    break;
                case Key.RIGHT:
                    MovePlayer(0, 1);
                    break;
                case Key.FIRE:
                    if (playerX + playerModel[0].Length < mapSizeX - 1)
                    {
                        currentPlayerBulletPos.Add(new List<int[]> { });

                        InsertModel(playerY + playerModel.GetLength(0) - (playerModel.GetLength(0) - 1) / 2 - 1, playerX + playerModel[0].Length, bullet, currentPlayerBulletPos.Last());

                        fireSound.Play();
                    }
                    break;
                case Key.END:
                    exit = true;
                    break;
            }

            SpawnEnemies(6, currentEnemyPos.Count);

            if (timerForMoveBullets >= 2)
            {
                timerForMoveBullets = 0;

                MoveBullet(currentPlayerBulletPos, bullet, false);
                MoveBullet(currentEnemyBulletPos, bulletE, true);
            }

            MoveEnemy(2);
            ShootEnemy();

            if (health <= 0)
                gameover = true;
        }
    }
}
