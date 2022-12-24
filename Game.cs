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
        static Random enemyPos = new Random();

        static string path = @"G:\Programming\Space Impact\files";
        static int mapSizeY = 30, mapSizeX = 70;
        static int playerY, playerX;

        static int timerForBullets = 0, timerForEnemies = 0, timerForMoveEnemies = 0;

        static ConsoleKeyInfo button;
        enum key { UP, DOWN, RIGHT, LEFT, FIRE, END, TRASH }
        static key pressed;

        static string[] bullet = new string[1] { "->" };
        static string[] bulletE = new string[1] { "<-" };

        static string[] playerModel = new string[File.ReadAllLines(path + @"\player.txt").GetLength(0)];
        static List<string[]> enemyModels = new List<string[]> { };

        static char[,] map = new char[mapSizeY, mapSizeX];

        static List<List<int[]>> currentEnemyPos = new List<List<int[]>> {};
        static List<int[]> currentPlayerPos = new List<int[]> { };
        static List<int[]> currentPlayerBulletPos = new List<int[]> { };

        static void Setup() //Первоначальная настройка
        {
            Console.SetWindowSize(100, 40);
            Console.CursorVisible = false;

            playerModel = File.ReadAllLines(path + @"\player.txt");

            enemyModels.Add(new string[File.ReadAllLines(path + @"\enemies\enemy0.txt").GetLength(0)]);
            enemyModels[0] = File.ReadAllLines(path + @"\enemies\enemy0.txt");

            playerY = mapSizeY / 2;
            playerX = mapSizeX / 8;

            InsertModel(playerY, playerX, playerModel, currentPlayerPos);

            for (int i = 0; i < mapSizeX + 3; i++)
                Console.Write("@");

            Console.WriteLine();

            for (int i = 0; i < mapSizeY; i++)
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
        static void Draw() //Отрисовка 
        {
            DrawModel(playerY, playerX, playerModel);
            for (int i = 0; i < currentPlayerBulletPos.Count; i += 2)
            {
                DrawModel(currentPlayerBulletPos[i][0], currentPlayerBulletPos[i][1], bullet);
            }
            foreach (var i in currentEnemyPos)
            {
                if(i.Count > 0)
                DrawModel(i[0][0],i[0][1],enemyModels[0]);
            }
        }
        static void Input() //Отслеживание ввода 
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
        static void Logic() //Логика игры
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

            SpawnEnemies(3, currentEnemyPos.Count);
            if (timerForBullets >= 2)
            {
                timerForBullets = 0;
                MoveBullet();
            }
            MoveEnemy(3);
            
        } 
        static void Main() //MAIN
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
        static void SpawnEnemies(int max, int countOfEnemy) //Спавн врагов 
        { 
            if(timerForEnemies >= 5 && countOfEnemy < max)
            {
                //enemyPos.Next(0, mapSizeY);
                int[] posY = new int [10];
                int posX = mapSizeX - enemyModels[0][0].Length - 1, y = 0; 
                bool spawn = false;
                if (currentEnemyPos.Count > 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        posY[j] = enemyPos.Next(0, mapSizeY - 1);
                    }

                    for (int j = 0; j < posY.Length && !spawn; j++)
                    {
                        foreach (var i in currentEnemyPos)
                        {
                            if (posY[j] + enemyModels[0].GetLength(0) < i[0][0] || posY[j] > i[i.Count - 1][0])
                            {
                                y = j;
                                spawn = true;
                            }
                            else if (posX > i[i.Count - 1][1])
                            {
                                y = j;
                                spawn = true;
                            }
                            else
                            {
                                spawn = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    posY[y] = enemyPos.Next(0, mapSizeY);
                    spawn = true;
                }

                if (spawn) {
                    currentEnemyPos.Add(new List<int[]> { });
                    timerForEnemies = 0;
                    InsertModel(posY[y], posX, enemyModels[0], currentEnemyPos.Last());
                }
            }
        }
        static void InsertModel(int y, int x, string[] Model, List<int[]> currentPos) //Добавить модель в игру (Игрок, пуля, враг)
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
        static void DeleteModel(int y, int x, string[] Model, List<int[]> currentPos, int firstBulCord = 0) //Удалить модель из игры 
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    map[y + i, x + j] = '\0';
                    Console.SetCursorPosition(x + j + 3, y + i + 1);
                    Console.Write("\b \b");
                }
            }
            if (Model.Equals(bullet))
            {
                if (firstBulCord < 0)
                    firstBulCord = 0;
                currentPos.RemoveAt(firstBulCord);
                currentPos.RemoveAt(firstBulCord);
            }
            else
                currentPos.Clear(); 
        }
        static void DrawModel(int y, int x, string[] Model) //Отрисовка моделей 
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
        static void MoveBullet() //Передвижение пуль 
         {
            for(int i = 0; i < currentPlayerBulletPos.Count; i += 2)
            {
                int y = currentPlayerBulletPos[0][0];
                int x = currentPlayerBulletPos[0][1];
                DeleteModel(y, x, bullet, currentPlayerBulletPos);
                if (x < (mapSizeX - 3))
                {
                    if (!HitCheck(y, x + 2, bullet, currentPlayerBulletPos))
                        InsertModel(y, x + 2, bullet, currentPlayerBulletPos);
                    else
                    {
                        SearchAndKill(y, x + 3);
                    }
                }
            }
        }
        static void MovePlayer(int modY, int modX) //Передвижение игрока 
        {
            if (playerY + modY + playerModel.GetLength(0) - 1 != mapSizeY && playerY + modY >= 0 && playerX + modX + playerModel[0].Length != mapSizeX && playerX + modX >= 0)
            {
                DeleteModel(playerY, playerX, playerModel, currentPlayerPos);
                InsertModel(playerY += modY, playerX += modX, playerModel, currentPlayerPos);
            }
        }
        static void MoveEnemy(int modX) //Передвижение врагов 
        {
            if (timerForMoveEnemies >= 6)
            {
                timerForMoveEnemies = 0;
                

                foreach (var i in currentEnemyPos.AsEnumerable().Reverse())
                {
                    int y = i[0][0];
                    int x = i[0][1];

                    if (x - modX >= 0)
                    {
                        DeleteModel(y, x, enemyModels[0], i);
                        if (!HitCheck(y, x - modX, enemyModels[0], i))
                            InsertModel(y, x - modX, enemyModels[0], i);
                        else
                        {
                            currentEnemyPos.Remove(i);
                            SearchAndKill(y, x - 2, true);
                        }
                    }
                    else
                    {
                        DeleteModel(i[0][0], i[0][1], enemyModels[0], i);
                        currentEnemyPos.Remove(i);
                    }
                }
            }
        }
        static bool HitCheck(int y, int x, string[] Model, List<int[]> currentPos) //Проверка попадания 
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    if(map[y + i, x + j] != '\0')
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static void SearchAndKill(int y, int x, bool enemy = false) //Найти модель по координатам и удалить из игры 
        {
            if (!enemy)
            {
                foreach (var i in currentEnemyPos)
                {
                    if(y >= i[0][0] && y <= i[i.Count - 1][0] && x >= i[0][1] && x <= i[i.Count - 1][1])
                    {
                        DeleteModel(i[0][0], i[0][1], enemyModels[0], i);
                        currentEnemyPos.Remove(i);
                        break;
                    }
                }
            }
            else
            {
                foreach (var i in currentPlayerBulletPos)
                {
                    if (y <= i[0] && i[0] <= y + enemyModels[0].GetLength(0) && x == i[1])
                    {
                        DeleteModel(i[0], x, bullet, currentPlayerBulletPos, currentPlayerBulletPos.IndexOf(i) - 1);
                        Console.SetCursorPosition(0, mapSizeY + 2);
                        break;
                    }
                }
                var a = 2 + 2;
            }
        }
        static void Timer(object o)
        {
            timerForBullets++;
            timerForEnemies++;
            timerForMoveEnemies++;
        }
    }
}