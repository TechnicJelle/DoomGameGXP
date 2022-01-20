using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Player : GameObject
	{
		public static Vector2 Position { get; private set; }
		public static float PlayerA { get; private set; }

		private const float F_ROTATION_SPEED = 0.003f;
		private const float F_MOVE_SPEED = 0.005f;

		public const float ViewDepth = 16.0f;

		public Player(float startX, float startY, float startAngle)
		{
			Position = new Vector2(startX, startY);
			PlayerA = startAngle;
		}

		public static void MoveInput()
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
				Position.x += Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				Position.y += Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				if (MyGame.Level.GetTileAtPosition((int) Position.x,(int) Position.y).Type == MyGame.TileType.Wall)
				{
					Position.x -= Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
					Position.y -= Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}

			if (Input.GetKey(Key.S))
			{
				Position.x -= Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				Position.y -= Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				if (MyGame.Level.GetTileAtPosition((int) Position.x,(int) Position.y).Type == MyGame.TileType.Wall)
				{
					Position.x += Mathf.Sin(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
					Position.y += Mathf.Cos(PlayerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}
		}
	}
}