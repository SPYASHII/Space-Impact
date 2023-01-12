using System;
using System.Threading;

namespace Space_Impact
{
    public partial class Game
    {
        public static void Main()
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
    }
}