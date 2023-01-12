using System;
using System.Collections.Generic;
using System.IO;

namespace Space_Impact
{
    public partial class Game
    {
        public static string path = @"..\\Debug\\files";

        public static System.Media.SoundPlayer fireSound = new System.Media.SoundPlayer(path + @"\\sounds\\fire.wav");
        public static System.Media.SoundPlayer gameOver = new System.Media.SoundPlayer(path + @"\\sounds\\gameover.wav");
        public static System.Media.SoundPlayer playerHit = new System.Media.SoundPlayer(path + @"\\sounds\\playerhit.wav");

        public static Random enemyPos = new Random();

        public static int mapSizeY = 18, mapSizeX = 50;
        public static int playerY, playerX, health, score;
        public static int timerForMoveBullets = 0, timerForEnemyShoot = 0, timerForEnemies = 0, timerForBigEnemies = 0, timerForMoveEnemies = 0;

        public static bool gameover = false, exit = false;

        public static ConsoleKeyInfo button;
        public enum Key { UP, DOWN, RIGHT, LEFT, FIRE, END, TRASH }
        public static Key pressed;

        public static string[] bullet = new string[1] { "->" };
        public static string[] bulletE = new string[1] { "<-" };

        public static string[] playerModel = new string[File.ReadAllLines(path + @"\\player.txt").GetLength(0)];
        public static List<string[]> enemyModels = new List<string[]> { };

        public static char[,] map = new char[mapSizeY, mapSizeX];

        public static List<List<int[]>> currentEnemyPos = new List<List<int[]>> { };
        public static List<int[]> currentPlayerPos = new List<int[]> { };

        public static List<List<int[]>> currentPlayerBulletPos = new List<List<int[]>> { };
        public static List<List<int[]>> currentEnemyBulletPos = new List<List<int[]>> { };
    }
}
