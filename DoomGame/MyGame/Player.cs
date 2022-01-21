using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Player : GameObject
	{
		public static Vector2 position { get; private set; }
		public static float playerA { get; private set; }

		private const float F_ROTATION_SPEED = 0.003f;
		private const float F_MOVE_SPEED = 0.005f;

		public const float VIEW_DEPTH = 16.0f;

		public Player(float startX, float startY, float startAngle)
		{
			position = new Vector2(startX, startY);
			playerA = startAngle;
		}

		public static void MoveInput()
		{
			if (Input.GetKey(Key.A))
			{
				playerA -= F_ROTATION_SPEED * Time.deltaTime;
			}

			if (Input.GetKey(Key.D))
			{
				playerA += F_ROTATION_SPEED * Time.deltaTime;
			}

			if (Input.GetKey(Key.W))
			{
				position.x += Mathf.Sin(playerA) * F_MOVE_SPEED * Time.deltaTime;
				position.y += Mathf.Cos(playerA) * F_MOVE_SPEED * Time.deltaTime;
				if (MyGame.level.GetTileAtPosition((int) position.x,(int) position.y).GetType() == typeof(TileWall))
				{
					position.x -= Mathf.Sin(playerA) * F_MOVE_SPEED * Time.deltaTime;
					position.y -= Mathf.Cos(playerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}

			if (Input.GetKey(Key.S))
			{
				position.x -= Mathf.Sin(playerA) * F_MOVE_SPEED * Time.deltaTime;
				position.y -= Mathf.Cos(playerA) * F_MOVE_SPEED * Time.deltaTime;
				if (MyGame.level.GetTileAtPosition((int) position.x,(int) position.y).GetType() == typeof(TileWall))
				{
					position.x += Mathf.Sin(playerA) * F_MOVE_SPEED * Time.deltaTime;
					position.y += Mathf.Cos(playerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}
		}
	}
}