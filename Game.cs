using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;

namespace SpaceImpact
{
    class Game
    {
        static string path = @"..\\Debug\\files";

        static System.Media.SoundPlayer fireSound = new System.Media.SoundPlayer(path + @"\\sounds\\fire.wav");
        static System.Media.SoundPlayer gameOver = new System.Media.SoundPlayer(path + @"\\sounds\\gameover.wav");
        static System.Media.SoundPlayer playerHit = new System.Media.SoundPlayer(path + @"\\sounds\\playerhit.wav");

        static Random enemyPos = new Random();

        static int mapSizeY = 18, mapSizeX = 50;
        static int playerY, playerX, health, score;
        static int timerForMoveBullets = 0, timerForEnemyShoot = 0, timerForEnemies = 0, timerForBigEnemies = 0, timerForMoveEnemies = 0;

        static bool gameover = false, exit = false;

        static ConsoleKeyInfo button;
        enum Key { UP, DOWN, RIGHT, LEFT, FIRE, END, TRASH }
        static Key pressed;

        static string[] bullet = new string[1] { "->" };
        static string[] bulletE = new string[1] { "<-" };

        static string[] playerModel = new string[File.ReadAllLines(path + @"\\player.txt").GetLength(0)];
        static List<string[]> enemyModels = new List<string[]> { };

        static char[,] map = new char[mapSizeY, mapSizeX];

        static List<List<int[]>> currentEnemyPos = new List<List<int[]>> {};
        static List<int[]> currentPlayerPos = new List<int[]> { };

        static List<List<int[]>> currentPlayerBulletPos = new List<List<int[]>> { };
        static List<List<int[]>> currentEnemyBulletPos = new List<List<int[]>> { };

        static void Main() //MAIN
        {
            Timer speedOfBullet = new Timer(Timer, null, 0, 50);
            while (!exit)
            {
                Menu();
                if (exit)
                    break;
                Setup();

                try
                {
                    while (!gameover && !exit)
                    {
                        Logic();
                        Draw();
                        Input();
                    }
                }
                catch (Exception exeption)
            {
                Console.Beep();
                Console.Clear();

                Console.WriteLine(exeption.Message);

                Console.ReadKey(true);
            }

            if (!exit && gameover)
                {
                    gameOver.Play();
                    Thread.Sleep(1000);

                    Console.ReadKey(true);
                    gameOver.Stop();
                }
            }
        }
        static void Menu() //Меню 
        {
            Console.CursorVisible = false;

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.SetWindowSize(67, mapSizeY + 5);

            string[] spaceImpact = new string[File.ReadAllLines(path + @"\enemies\enemy0.txt").GetLength(0)];
            spaceImpact = File.ReadAllLines(path + @"\spaceimpact.txt");

            Console.SetCursorPosition(0, 3);

            foreach(var i in spaceImpact)
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
        static void Setup() //Первоначальная настройка
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

        static void Draw() //Отрисовка 
        {
            DrawModel(playerY, playerX, playerModel);

            foreach (var i in currentPlayerBulletPos)
            {
                DrawModel(i[0][0], i[0][1], bullet);
            }

            foreach (var i in currentEnemyBulletPos)
            {
                DrawModel(i[0][0], i[0][1], bulletE);
            }

            foreach (var i in currentEnemyPos)
            {
                DrawModel(i[0][0],i[0][1],enemyModels[i.Last()[0]]);
            }

            DrawDelHealth();
            DrawScore();
             
            if(gameover)
            {
                Console.Clear();

                Console.SetCursorPosition(Console.WindowWidth/2 - 5,Console.WindowHeight/2);
                Console.Write("Game Over!");

                Console.SetCursorPosition(Console.WindowWidth / 2 - 7, Console.WindowHeight / 2 + 1);
                Console.Write($"Your Score: {score}");
            }
        }
        static void Input() //Отслеживание ввода 
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
        static void Logic() //Логика игры
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

        static void SpawnEnemies(int max, int countOfEnemy) //Спавн врагов 
        { 
            if(timerForEnemies >= 5 && countOfEnemy < max)
            {
                int enemyType = 0, y = 0;
                int[] posY = new int [10];

                bool spawn = false;

                if (timerForBigEnemies >= 20)
                {
                    enemyType = 1;
                    timerForBigEnemies = 0;
                }

                int posX = mapSizeX - enemyModels[enemyType][0].Length - 1; 
                
                if (currentEnemyPos.Count > 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        posY[j] = enemyPos.Next(0, mapSizeY - 2);
                    }

                    for (int j = 0; j < posY.Length && !spawn; j++)
                    {
                        foreach (var i in currentEnemyPos)
                        {
                            if (posY[j] + enemyModels[enemyType].GetLength(0) < i[0][0] || posY[j] > i[i.Count - 2][0])
                            {
                                y = j;
                                spawn = true;
                            }
                            else if (posX > i[i.Count - 2][1])
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
                    posY[y] = enemyPos.Next(0, mapSizeY - enemyModels[enemyType].GetLength(0));
                    spawn = true;
                }

                if (spawn) 
                {
                    timerForEnemies = 0;

                    currentEnemyPos.Add(new List<int[]> { });
                    InsertModel(posY[y], posX, enemyModels[enemyType], currentEnemyPos.Last());

                    currentEnemyPos.Last().Add(new int[2] {enemyType,enemyType}); //Добавляем в конец координат каждого врага его тип
                }
            }
        }
        static void MoveEnemy(int modX) //Передвижение врагов 
        {
            if (timerForMoveEnemies >= 2)
            {
                int modXBuf = modX;
                int modY;

                timerForMoveEnemies = 0;

                foreach (var i in currentEnemyPos.AsEnumerable().Reverse())
                {
                    modX = modXBuf;

                    if (enemyPos.Next() % 2 == 0)
                        modY = 1;
                    else
                        modY = -1;

                    int y = i[0][0];
                    int x = i[0][1];

                    if (i.Last()[0] == 0)
                        modY = 0;
                    else if (y + modY >= mapSizeY - 3 || y + modY <= 0)
                        modY *= -1;
                    else
                        modX = 1;

                    int enemyType = i.Last()[0];

                    if (x - modX >= 0)
                    {
                        DeleteModel(y, x, enemyModels[enemyType], i);

                        if (!HitCheck(y + modY, x - modX, enemyModels[enemyType], i))
                        {
                            InsertModel(y + modY, x - modX, enemyModels[enemyType], i);

                            i.Add(new int[2] { enemyType, enemyType });
                        }
                        else if (SearchAndKill(y + modY, x - modX, enemyModels[enemyType],true, false, enemyType))
                        {
                            currentEnemyPos.Remove(i);
                        }
                        else
                        {
                            InsertModel(y + modY, x - modX, enemyModels[enemyType], i);

                            i.Add(new int[2] { enemyType, enemyType });
                        }
                    }
                    else
                    {
                        DeleteModel(i[0][0], i[0][1], enemyModels[enemyType], i);
                        currentEnemyPos.Remove(i);
                    }
                }
            }
        }
        static void ShootEnemy() //Логика для выстрелов врагов
        {
            if(timerForEnemyShoot >= 10)
            {
                timerForEnemyShoot = 0;

                foreach(var i in currentEnemyPos)
                {
                    if(i.Last()[0] == 1)
                    {
                        if (i[0][1] - 1 > 1)
                        {
                            currentEnemyBulletPos.Add(new List<int[]> { });
                            InsertModel(i[0][0] + enemyModels[i.Last()[0]].GetLength(0) - (enemyModels[i.Last()[0]].GetLength(0) - 1) / 2 - 1, i[0][1] - 1, bulletE, currentEnemyBulletPos.Last());
                        }
                    }
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
        static void DeleteModel(int y, int x, string[] Model, List<int[]> currentPos) //Удалить модель из игры 
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    map[y + i, x + j] = '\0';
                    Console.SetCursorPosition(x + j + 3, y + i + 1 + 3);
                    Console.Write("\b \b");
                }
            }

            currentPos.Clear(); 
        }

        static void DrawModel(int y, int x, string[] Model) //Отрисовка моделей 
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    Console.SetCursorPosition(x + j + 2, y + i + 1 + 3);
                    Console.Write(map[y + i, x + j]);
                }
            }
        }
        static void DrawDelHealth(bool draw = true) //Отрисовка/Удаление HP
        {
            if (health < 0)
                health = 0;

            if (draw)
            {
                for (int i = 0; i < health; i++)
                {
                    Console.SetCursorPosition(i * 6, 0);
                    Console.Write("//\\//\\");
                    Console.SetCursorPosition(i * 6, 1);
                    Console.Write("\\\\   /");
                    Console.SetCursorPosition(i * 6, 2);
                    Console.Write(" \\\\_/");
                }
            }
            else
            {
                Console.SetCursorPosition((health) * 6, 0);
                Console.Write("\b       \b");
                Console.SetCursorPosition((health) * 6, 1);
                Console.Write("\b       \b");
                Console.SetCursorPosition((health) * 6, 2);
                Console.Write("\b      \b");
            }
        }
        static void DrawScore() //Отрисовка счета
        {
            Console.SetCursorPosition(mapSizeX/2 - 5, 2);
            Console.Write($"YOUR SCORE: {score}");
        }
        static void DrawMap() //Отрисовка карты
        {
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < mapSizeX + 3; i++)
                    Console.Write(" ");
                Console.WriteLine();
            }

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
        static void MoveBullet(List<List<int[]>> currentBulletPos, string[] bullet, bool enemy) //Передвижение пуль 
         {
            int modX = 1;

            if(enemy)
                modX = -1;

            foreach (var i in currentBulletPos.AsEnumerable().Reverse())
            {
                int y = i[0][0];
                int x = i[0][1];

                DeleteModel(y, x, bullet, i);

                if ((!enemy && x < (mapSizeX - 3)) || (enemy && x > 3))
                {
                    if (!HitCheck(y, x + 2 * modX, bullet, i))
                        InsertModel(y, x + 2 * modX, bullet, i);
                    else
                    {
                        currentBulletPos.Remove(i);

                        SearchAndKill(y, x + 3 * modX, bullet,enemy);
                    }
                }
                else
                    currentBulletPos.Remove(i);
            }
        }
        static void MovePlayer(int modY, int modX) //Передвижение игрока 
        {
            if (playerY + modY + playerModel.GetLength(0) - 1 != mapSizeY && playerY + modY >= 0 && playerX + modX + playerModel[0].Length != mapSizeX && playerX + modX >= 0)
            {
                DeleteModel(playerY, playerX, playerModel, currentPlayerPos);

                if (HitCheck(playerY + modY, playerX + modX, playerModel, currentPlayerPos)) //Проверка на то врезался ли игрок во врага или пулю
                    SearchAndKill(playerY + modY, playerX + modX, playerModel,false, true);

                InsertModel(playerY += modY, playerX += modX, playerModel, currentPlayerPos);
            }
        }
        
        static bool HitCheck(int y, int x, string[] Model, List<int[]> currentPos) //Проверка на наличие обьектов по координатам y,x учитывая модель которая появится по этим координатам 
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
        static bool SearchAndKill(int y, int x, string[] model, bool enemy = false, bool player = false, int enemyType = 100) //Найти модель по координатам и удалить из игры 
        {
            bool hit = false;
            if (!enemy)
            {
                if (!player)
                {
                    foreach (var i in currentEnemyPos)
                    {
                        if (y >= i[0][0] && y <= i[i.Count - 2][0] && x >= i[0][1] && x <= i[i.Count - 2][1])
                        {
                            Score(i.Last()[0]);

                            DeleteModel(i[0][0], i[0][1], enemyModels[i.Last()[0]], i);
                            currentEnemyPos.Remove(i);

                            break;
                        }
                    }
                    foreach (var i in currentEnemyBulletPos)
                    {
                        if (y >= i[0][0] && y <= i[i.Count - 1][0] && x >= i[0][1] && x <= i[i.Count - 1][1])
                        {
                            DeleteModel(i[0][0], i[0][1], bulletE, i);
                            currentEnemyBulletPos.Remove(i);

                            break;
                        }
                    }
                }
                else 
                {
                    foreach (var i in currentEnemyPos)
                    {
                        if (y <= i[i.Count - 2][0] && x <= i[i.Count - 2][1] && y + playerModel.GetLength(0) >= i[0][0] && x + playerModel[0].Length >= i[0][1])
                        {
                            DeleteModel(i[0][0], i[0][1], enemyModels[i.Last()[0]], i);
                            currentEnemyPos.Remove(i);

                            health -= 1;
                            playerHit.Play();

                            DrawDelHealth(false);
                            break;
                        }
                    }
                    foreach (var i in currentEnemyBulletPos)
                    {
                        if (y <= i[i.Count - 1][0] && x <= i[i.Count - 1][1] && y + playerModel.GetLength(0) >= i[0][0] && x + playerModel[0].Length >= i[0][1])
                        {
                            DeleteModel(i[0][0], i[0][1], bulletE, i);
                            currentEnemyBulletPos.Remove(i);

                            health -= 1;
                            playerHit.Play();

                            DrawDelHealth(false);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var i in currentPlayerBulletPos)
                {
                    if ((y <= i[0][0] && i[0][0] <= y + model.GetLength(0)) && (x <= i[1][1] || x <= i[0][1]))
                    {
                        DeleteModel(i[0][0], i[0][1], bullet, i);
                        currentPlayerBulletPos.Remove(i);

                        Score(enemyType);

                        hit = true;
                        break;
                    }
                }
                if (!hit)
                {
                    if (y <= currentPlayerPos[currentPlayerPos.Count - 1][0] && x <= currentPlayerPos[currentPlayerPos.Count - 1][1] && y + model.GetLength(0) >= currentPlayerPos[0][0] && x + model[0].Length >= currentPlayerPos[0][1]) {
                        hit = true;

                        health -= 1;
                        DrawDelHealth(false);

                        playerHit.Play();
                    }
                }    
            }

            return hit;

            void Score (int eType) //Увеличение счета игрока взависимости от того какого врага он убил
            {
                switch (eType)
                {
                    case 0:
                        score += 10;
                        break;
                    case 1:
                        score += 25;
                        break;
                }
            }
        }

        static void Timer(object o)
        {
            timerForMoveBullets++;
            timerForEnemyShoot++;
            timerForEnemies++;
            timerForMoveEnemies++;
            timerForBigEnemies++;
        }
    }
}