namespace Space_Impact
{
    public partial class Game
    {
        public static void MovePlayer(int modY, int modX) //Передвижение игрока 
        {
            if (playerY + modY + playerModel.GetLength(0) - 1 != mapSizeY && playerY + modY >= 0 && playerX + modX + playerModel[0].Length != mapSizeX && playerX + modX >= 0)
            {
                DeleteModel(playerY, playerX, playerModel, currentPlayerPos);

                if (HitCheck(playerY + modY, playerX + modX, playerModel, currentPlayerPos)) //Проверка на то врезался ли игрок во врага или пулю
                    SearchAndKill(playerY + modY, playerX + modX, playerModel, false, true);

                InsertModel(playerY += modY, playerX += modX, playerModel, currentPlayerPos);
            }
        }
    }
}
