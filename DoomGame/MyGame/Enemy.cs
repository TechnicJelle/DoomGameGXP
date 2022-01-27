using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Enemy : Renderable
	{
		public Vector2 enemyPosition => position;
		public Vector2 edge1 => p1;
		public Vector2 edge2 => p2;

		private Sprite minimapTexture { get; }

		private const float MOVE_STEP = 0.01f;

		public Enemy(string filename, float x, float y) : base(filename)
		{
			position = new Vector2(x, y);
			minimapTexture = new Sprite(filename, true, false);
		}

		public void Move()
		{
			float xMove = Utils.Random(-MOVE_STEP, MOVE_STEP) * Time.deltaTime;
			float yMove = Utils.Random(-MOVE_STEP, MOVE_STEP) * Time.deltaTime;

			if (MyGame.currentLevel.GetTileAtPosition((int)(position.x + xMove), (int)(position.y + yMove)).GetType()
			    == typeof(TileWall)) return;
			position.x += xMove;
			position.y += yMove;
		}

		public override void RefreshVisuals()
		{
			normal = MyGame.currentLevel.player.heading.Copy().Mult(-1.0f);

			Minimap.DrawEnemy(minimapTexture, position);

			CalculatePointsInWorldSpace();
			base.RefreshVisuals();
		}
	}
}