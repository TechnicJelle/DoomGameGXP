namespace GXPEngine.MyGame
{
	public class Player
	{
		public static float PlayerX;
		public static float PlayerY;
		public static float PlayerA;

		private const float F_ROTATION_SPEED = 0.003f;
		private const float F_MOVE_SPEED = 0.005f;

		public Player(float startX, float startY, float startAngle)
		{
			PlayerX = startX;
			PlayerY = startY;
			PlayerA = startAngle;
		}

		public void MoveInput()
		{
			if (Input.GetKey(Key.A))
			{
				PlayerA -= F_ROTATION_SPEED * Time.deltaTime;
			}

			if (Input.GetKey(Key.D))
			{
				PlayerA += F_ROTATION_SPEED * Time.deltaTime;
			}

			if (Input.GetKey(Key.W))
			{
				PlayerX += Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				PlayerY += Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				if (MyGame.Level.GetTileAtPosition((int) PlayerX,(int) PlayerY).Type == MyGame.TileType.Wall)
				{
					PlayerX -= Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
					PlayerY -= Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}

			if (Input.GetKey(Key.S))
			{
				PlayerX -= Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				PlayerY -= Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				if (MyGame.Level.GetTileAtPosition((int) PlayerX,(int) PlayerY).Type == MyGame.TileType.Wall)
				{
					PlayerX += Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
					PlayerY += Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}
		}
	}
}