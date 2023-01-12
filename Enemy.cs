using System.Collections.Generic;
using System.Linq;

namespace Space_Impact
{
    public partial class Game
    {
        public static void SpawnEnemies(int max, int countOfEnemy) //Спавн врагов 
        {
            if (timerForEnemies >= 5 && countOfEnemy < max)
            {
                int enemyType = 0, y = 0;
                int[] posY = new int[10];

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

                    currentEnemyPos.Last().Add(new int[2] { enemyType, enemyType }); //Добавляем в конец координат каждого врага его тип
                }
            }
        }
        public static void MoveEnemy(int modX) //Передвижение врагов 
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
                        else if (SearchAndKill(y + modY, x - modX, enemyModels[enemyType], true, false, enemyType))
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
        public static void ShootEnemy() //Логика для выстрелов врагов
        {
            if (timerForEnemyShoot >= 10)
            {
                timerForEnemyShoot = 0;

                foreach (var i in currentEnemyPos)
                {
                    if (i.Last()[0] == 1)
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
    }
}
