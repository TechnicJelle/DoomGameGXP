using System;

namespace GXPEngine.MyGame
{
	public class Level
	{
		private readonly Tile[,] _tiles;
		public int TilesWidth { get; }
		public int TilesHeight { get; }

		private readonly (float, float)[] _p = new (float, float)[4]; // distance, dot product
		
		public Level(int w, int h, string mapContent)
		{
			TilesWidth = w;
			TilesHeight = h;
			if (mapContent.Length != TilesWidth * TilesHeight) throw new Exception();
			_tiles = new Tile[TilesWidth, TilesHeight];
			for (int ix = 0; ix < _tiles.GetLength(0); ix++)
			for (int iy = 0; iy < _tiles.GetLength(1); iy++)
			{
				switch (mapContent[iy * TilesWidth + ix])
				{
					case '#':
						_tiles[ix, iy] = new Tile(MyGame.TileType.Wall);
						break;
					case '.':
						_tiles[ix, iy] = new Tile(MyGame.TileType.Empty);
						break;
				}
			}
		}

		public void Render(EasyDraw canvas)
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

					if (nTestX < 0 || nTestX >= TilesWidth | nTestY < 0 || nTestY >= TilesHeight)
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

				float a = fRayAngle - Player.PlayerA;
				fDistanceToWall *= Mathf.Cos(a);

				float fCeiling = MyGame.Height / 2.0f - MyGame.Height / fDistanceToWall;
				float fFloor = MyGame.Height - fCeiling;

				if (bBoundary)
				{
					canvas.Stroke(0);
				}
				else
				{
					//Inverse Square(ish) Law:
					const float exp = 1.6f;
					float sq = Mathf.Pow(fDistanceToWall, exp);
					float wSq = Mathf.Pow(MyGame.ViewDepth, exp);
					int brightness = (int)MyGame.Map(sq, 0, wSq, 255, 0);
					canvas.Stroke(brightness);
					// canvas.Stroke((int) MyGame.Map(fDistanceToWall, 0, MyGame.ViewDepth, 255, 0));
				}

				canvas.Line(ix, fCeiling, ix, fFloor);
			}
		}

		public Tile GetTileAtPosition(int x, int y)
		{
			return _tiles[x, y];
		}
		
	}
}