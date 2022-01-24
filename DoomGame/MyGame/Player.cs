using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Player : GameObject
	{
		public static Vector2 position { get; private set; }
		public static float angle { get; private set; }
		public static Vector2 heading { get; private set; }

		private const float ROTATION_SPEED = 0.003f;
		private const float MOVE_SPEED = 0.005f;

		public const float VIEW_DEPTH = 16.0f;

		public Player(float startX, float startY, float startAngle)
		{
			position = new Vector2(startX, startY);
			Rotate(startAngle);
		}

		private static void Rotate(float amt)
		{
			angle += amt;
			heading = Vector2.FromAngle(angle);
		}

		public static void MoveInput()
		{
			if (Input.GetKey(Key.A))
			{
				Rotate(-ROTATION_SPEED * Time.deltaTime);
			}

			if (Input.GetKey(Key.D))
			{
				Rotate(ROTATION_SPEED * Time.deltaTime);
			}

			//TODO: Implement DDA here

			if (Input.GetKey(Key.W))
			{
				position.x += Mathf.Cos(angle) * MOVE_SPEED * Time.deltaTime;
				position.y += Mathf.Sin(angle) * MOVE_SPEED * Time.deltaTime;
				if (MyGame.level.GetTileAtPosition((int) position.x,(int) position.y).GetType() == typeof(TileWall))
				{
					position.x -= Mathf.Cos(angle) * MOVE_SPEED * Time.deltaTime;
					position.y -= Mathf.Sin(angle) * MOVE_SPEED * Time.deltaTime;
				}

				// (Vector2 intersection, float dist) result =
				// 	TileWall.DDA(position, Vector2.Add(position, Vector2.FromAngle(playerA).Mult(F_MOVE_SPEED * Time.deltaTime)), 1000.0f);
				// if (result.intersection != null)
				// {
				// 	Minimap.DebugFill(255, 0, 255);
				// 	Minimap.DebugCircle(result.intersection.x, result.intersection.y, 4);
				// }
				//
				// Console.WriteLine(result);
			}

			if (Input.GetKey(Key.S))
			{
				position.x -= Mathf.Cos(angle) * MOVE_SPEED * Time.deltaTime;
				position.y -= Mathf.Sin(angle) * MOVE_SPEED * Time.deltaTime;
				if (MyGame.level.GetTileAtPosition((int) position.x,(int) position.y).GetType() == typeof(TileWall))
				{
					position.x += Mathf.Cos(angle) * MOVE_SPEED * Time.deltaTime;
					position.y += Mathf.Sin(angle) * MOVE_SPEED * Time.deltaTime;
				}
			}
		}
	}
}