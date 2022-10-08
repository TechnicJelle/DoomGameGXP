using GXPEngine.GXPEngine.AddOns;
using GXPEngine.GXPEngine.Core;
using GXPEngine.GXPEngine.Utils;

namespace GXPEngine.MyGame;

public class Player
{
	public Vector2 Position { get; private set; }
	public float Angle { get; private set; }
	public Vector2 Heading { get; private set; }

	private const float ROTATION_SPEED = 0.003f;
	private const float MOVE_SPEED = 0.005f;
	private const float WALL_PADDING = 0.3f;

	public const float VIEW_DEPTH = 4.0f;

	public Player(float startX, float startY, float startAngle)
	{
		Position = new Vector2(startX, startY);
		Angle = 0.0f;
		Rotate(startAngle);
	}

	private void Rotate(float amt)
	{
		Angle += amt;
		Heading = Vector2.FromAngle(GXPEngine.Core.Angle.FromRadians(Angle));
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

		Vector2? moveDir = null;
		if (Input.GetKey(Settings.Up))
		{
			moveDir = Heading;
			moved = true;
		}
		else
		if (Input.GetKey(Settings.Down))
		{
			moveDir = Heading * -1.0f;
			moved = true;
		}

		if (moved && Settings.Minimap)
			Minimap.UpdatePlayer();

		if (moveDir == null) return;
		(TileWall tileWall, Vector2? _, float dist) = TileWall.DDA(Position, moveDir.Value, 1.0f);
		Position += (moveDir * (dist - WALL_PADDING)).Value.Limit(MOVE_SPEED * Time.deltaTime);
		if(tileWall != null && tileWall.GetType() == typeof(TileNext) && dist < WALL_PADDING * 1.1f)
			myGame?.NextLevel();
	}
}
