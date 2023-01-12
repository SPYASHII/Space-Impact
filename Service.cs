using System;
using System.Collections.Generic;
using System.Linq;

namespace Space_Impact
{
    public partial class Game
    {
        public static void InsertModel(int y, int x, string[] Model, List<int[]> currentPos) //Добавить модель в игру (Игрок, пуля, враг)
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
        public static void DeleteModel(int y, int x, string[] Model, List<int[]> currentPos) //Удалить модель из игры 
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

        public static bool HitCheck(int y, int x, string[] Model, List<int[]> currentPos) //Проверка на наличие обьектов по координатам y,x учитывая модель которая появится по этим координатам 
        {
            for (int i = 0; i < Model.GetLength(0); i++)
            {
                for (int j = 0; j < Model[i].Length; j++)
                {
                    if (map[y + i, x + j] != '\0')
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool SearchAndKill(int y, int x, string[] model, bool enemy = false, bool player = false, int enemyType = 100) //Найти модель по координатам и удалить из игры 
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
                    if (y <= currentPlayerPos[currentPlayerPos.Count - 1][0] && x <= currentPlayerPos[currentPlayerPos.Count - 1][1] && y + model.GetLength(0) >= currentPlayerPos[0][0] && x + model[0].Length >= currentPlayerPos[0][1])
                    {
                        hit = true;

                        health -= 1;
                        DrawDelHealth(false);

                        playerHit.Play();
                    }
                }
            }

            return hit;

            void Score(int eType) //Увеличение счета игрока взависимости от того какого врага он убил
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

        public static void MoveBullet(List<List<int[]>> currentBulletPos, string[] bullet, bool enemy) //Передвижение пуль 
        {
            int modX = 1;

            if (enemy)
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

                        SearchAndKill(y, x + 3 * modX, bullet, enemy);
                    }
                }
                else
                    currentBulletPos.Remove(i);
            }
        }

        public static void Timer(object o)
        {
            timerForMoveBullets++;
            timerForEnemyShoot++;
            timerForEnemies++;
            timerForMoveEnemies++;
            timerForBigEnemies++;
        }
    }
}
