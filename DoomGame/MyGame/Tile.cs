namespace GXPEngine.MyGame
{
	public class Tile
	{
		public MyGame.TileType Type { get; }
		public bool Visible = false;
		public float LastCalculatedDistanceToPlayer = float.MaxValue;
		public int X { get; }
		public int Y { get; }

		public Tile(MyGame.TileType t, int x, int y)
		{
			Type = t;
			X = x;
			Y = y;
		}
	}
}