using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceImpact
{
    class Game
    {
        static ConsoleKeyInfo button;
        enum key { UP, DOWN, RIGHT, LEFT, FIRE, END, TRASH}
        static key pressed;

        static int count = 1;
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
                    
                    break;
                case key.DOWN:
                    
                    break;
                case key.LEFT:
                    if (count > 0)
                    count--;
                    break;
                case key.RIGHT:
                    count++;
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
            while(true)
            {
                Logic();
                Draw();
                Input();
            }
        }
    }
}