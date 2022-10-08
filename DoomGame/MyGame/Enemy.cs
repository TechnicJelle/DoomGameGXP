using GXPEngine.GXPEngine;
using GXPEngine.GXPEngine.AddOns;
using GXPEngine.GXPEngine.Core;
using GXPEngine.GXPEngine.Utils;

namespace GXPEngine.MyGame;

public class Enemy : Renderable
{
	public Vector2 EnemyPosition => Position;
	public Vector2 Edge1 => P1;
	public Vector2 Edge2 => P2;

	private Sprite MinimapTexture { get; }

	private const float MOVE_STEP = 0.01f;

	public Enemy(string filename, float x, float y) : base(filename)
	{
		Position = new Vector2(x, y);
		MinimapTexture = new Sprite(filename, true, false);
	}

	public void Move()
	{
		float xMove = Utils.Random(-MOVE_STEP, MOVE_STEP) * Time.deltaTime;
		float yMove = Utils.Random(-MOVE_STEP, MOVE_STEP) * Time.deltaTime;

		if (MyGame.CurrentLevel.GetTileAtPosition((int)(Position.x + xMove), (int)(Position.y + yMove)).GetType()
		    == typeof(TileWall)) return;
		Position.x += xMove;
		Position.y += yMove;
	}

	public override void RefreshVisuals()
	{
		Normal = MyGame.CurrentLevel.Player.Heading * -1.0f;

		if (Settings.Minimap)
			Minimap.DrawEnemy(MinimapTexture, Position);

		CalculatePointsInWorldSpace();
		base.RefreshVisuals();
	}
}
