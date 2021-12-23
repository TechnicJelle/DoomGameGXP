using System;

namespace GXPEngine.MyGame
{
	public class Level
	{
		private readonly Tile[,] _tiles;
		private readonly int _tilesWidth;
		private readonly int _tilesHeight;

		private readonly (float, float)[] _p = new (float, float)[4]; // distance, dot product
		
		public Level(int w, int h, string mapContent)
		{
			_tilesWidth = w;
			_tilesHeight = h;
			if (mapContent.Length != _tilesWidth * _tilesHeight) throw new Exception();
			_tiles = new Tile[_tilesWidth, _tilesHeight];
			for (int ix = 0; ix < _tiles.GetLength(0); ix++)
			for (int iy = 0; iy < _tiles.GetLength(1); iy++)
			{
				switch (mapContent[ix * _tilesWidth + iy])
				{
					case '#':
						_tiles[ix, iy] = new Tile(MyGame.TileType.Wall);
						Console.WriteLine("wall");
						break;
					case '.':
						_tiles[ix, iy] = new Tile(MyGame.TileType.Empty);
						Console.WriteLine("empty");
						break;
				}
			}
		}

		public void Render()
		{
			//Render Walls
			for (int ix = 0; ix < MyGame.Width; ix++)
			{
				float fRayAngle = (Player.PlayerA - MyGame.FieldOfView / 2.0f) + (ix / (float) MyGame.Width) * MyGame.FieldOfView;

				float fDistanceToWall = 0.0f;
				bool bHitWall = false;
				bool bBoundary = false;

				float fEyeX = Mathf.Sin(fRayAngle);
				float fEyeY = Mathf.Cos(fRayAngle);

				while (!bHitWall && fDistanceToWall < MyGame.ViewDepth)
				{
					fDistanceToWall += 0.01f;

					int nTestX = (int) (Player.PlayerX + fEyeX * fDistanceToWall);
					int nTestY = (int) (Player.PlayerY + fEyeY * fDistanceToWall);

					if (nTestX < 0 || nTestX >= _tilesWidth | nTestY < 0 || nTestY >= _tilesHeight)
					{
						//Ray has gone out of map bounds
						bHitWall = true;
						fDistanceToWall = MyGame.ViewDepth;
					}
					else
					{
						if (GetTileAtPosition(nTestX, nTestY).Type == MyGame.TileType.Empty) continue;
						bHitWall = true;

						for (int tx = 0; tx < 2; tx++)
						{
							for (int ty = 0; ty < 2; ty++)
							{
								float vy = (float) nTestY + ty - Player.PlayerY;
								float vx = (float) nTestX + tx - Player.PlayerX;
								float d = Mathf.Sqrt(vx * vx + vy * vy);
								float dot = (fEyeX * vx / d) + (fEyeY * vy / d);
								_p[(tx * 2) + ty] = (d, dot);
							}
						}

						Array.Sort(_p, (left, right) => left.Item1.CompareTo(right.Item1));

						const float fBound = 0.001f;
						if (Mathf.Acos(_p[0].Item2) < fBound) bBoundary = true;
						if (Mathf.Acos(_p[1].Item2) < fBound) bBoundary = true;
					}
				}

				float fCeiling = MyGame.Height / 2.0f - MyGame.Height / fDistanceToWall;
				float fFloor = MyGame.Height - fCeiling;

				if (bBoundary)
				{
					MyGame.Canvas.Stroke(0);
				}
				else
				{
					MyGame.Canvas.Stroke((int) MyGame.Map(fDistanceToWall, 0, MyGame.ViewDepth, 255, 0));
				}

				MyGame.Canvas.Line(ix, fCeiling, ix, fFloor);
			}
		}

		public Tile GetTileAtPosition(int x, int y)
		{
			return _tiles[x, y];
		}
		
	}
}