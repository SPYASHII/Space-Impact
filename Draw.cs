using System;
using System.Linq;

namespace Space_Impact
{
    public partial class Game
    {
        public static void Draw() //Отрисовка 
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
                DrawModel(i[0][0], i[0][1], enemyModels[i.Last()[0]]);
            }

            DrawDelHealth();
            DrawScore();

            if (gameover)
            {
                Console.Clear();

                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2);
                Console.Write("Game Over!");

                Console.SetCursorPosition(Console.WindowWidth / 2 - 7, Console.WindowHeight / 2 + 1);
                Console.Write($"Your Score: {score}");
            }
        }

        public static void DrawModel(int y, int x, string[] Model) //Отрисовка моделей 
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
        public static void DrawDelHealth(bool draw = true) //Отрисовка/Удаление HP
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
        public static void DrawScore() //Отрисовка счета
        {
            Console.SetCursorPosition(mapSizeX / 2 - 5, 2);
            Console.Write($"YOUR SCORE: {score}");
        }
        public static void DrawMap() //Отрисовка карты
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
    }
}
