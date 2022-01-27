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
		private const float WALL_PADDING = 0.3f;

		public const float VIEW_DEPTH = 4.0f;

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
			bool moved = false; //TODO: Find a slightly better solution for this
			if (Input.GetKey(Key.A))
			{
				Rotate(-ROTATION_SPEED * Time.deltaTime);
				moved = true;
			}

			if (Input.GetKey(Key.D))
			{
				Rotate(ROTATION_SPEED * Time.deltaTime);
				moved = true;
			}

			Vector2 moveDir = null;
			if (Input.GetKey(Key.W))
			{
				moveDir = heading.Copy();
				moved = true;
			}
			else
			if (Input.GetKey(Key.S))
			{
				moveDir = heading.Copy().Mult(-1.0f);
				moved = true;
			}

			if (moved)
				Minimap.UpdatePlayer();

			if (moveDir == null) return;
			(TileWall _, Vector2 _, float dist) = TileWall.DDA(position, moveDir, 1.0f);
			position.Add(Vector2.Mult(moveDir, dist - WALL_PADDING).Limit(MOVE_SPEED * Time.deltaTime));
		}
	}
}