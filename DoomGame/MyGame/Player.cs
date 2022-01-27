using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Player
	{
		public Vector2 position { get; }
		public float angle { get; private set; }
		public Vector2 heading { get; private set; }

		private const float ROTATION_SPEED = 0.003f;
		private const float MOVE_SPEED = 0.005f;
		private const float WALL_PADDING = 0.3f;

		public const float VIEW_DEPTH = 4.0f;

		public Player(float startX, float startY, float startAngle)
		{
			position = new Vector2(startX, startY);
			angle = 0.0f;
			Rotate(startAngle);
		}

		private void Rotate(float amt)
		{
			angle += amt;
			heading = Vector2.FromAngle(angle);
		}

		public void MoveInput(MyGame myGame = null)
		{
			bool moved = false; //TODO: Find a slightly better solution for this
			if (Input.GetKey(Settings.Left))
			{
				Rotate(-ROTATION_SPEED * Time.deltaTime);
				moved = true;
			}

			if (Input.GetKey(Settings.Right))
			{
				Rotate(ROTATION_SPEED * Time.deltaTime);
				moved = true;
			}

			Vector2 moveDir = null;
			if (Input.GetKey(Settings.Up))
			{
				moveDir = heading.Copy();
				moved = true;
			}
			else
			if (Input.GetKey(Settings.Down))
			{
				moveDir = heading.Copy().Mult(-1.0f);
				moved = true;
			}

			if (moved && Settings.Minimap)
				Minimap.UpdatePlayer();

			if (moveDir == null) return;
			(TileWall tileWall, Vector2 _, float dist) = TileWall.DDA(position, moveDir, 1.0f);
			position.Add(Vector2.Mult(moveDir, dist - WALL_PADDING).Limit(MOVE_SPEED * Time.deltaTime));
			if(tileWall != null && tileWall.GetType() == typeof(TileNext) && dist < WALL_PADDING * 1.1f)
				myGame?.NextLevel();
		}
	}
}