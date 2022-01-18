using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Level
	{
		private readonly Tile[,] _tiles;
		public int TilesWidth { get; }
		public int TilesHeight { get; }

		private readonly (float, float)[] _p = new (float, float)[4]; // distance, dot product

		private List<Tile> _visibleTiles;

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
						_tiles[ix, iy] = new Tile(MyGame.TileType.Wall, ix, iy);
						break;
					case '.':
						_tiles[ix, iy] = new Tile(MyGame.TileType.Empty, ix, iy);
						break;
				}
			}
		}

		public void Render(EasyDraw canvas, Minimap minimap)
		{
			//Reset all visible tiles
			_visibleTiles = new List<Tile>();
			for (int ix = 0; ix < _tiles.GetLength(0); ix++)
			for (int iy = 0; iy < _tiles.GetLength(1); iy++)
			{
				_tiles[ix, iy].Visible = false;
			}

			//Find tiles to render
			//For every x pixel, send out a ray that goes until it has hit a wall or reached the maximum render distance
			for (int ix = 0; ix < MyGame.Width; ix++)
			{
				float fRayAngle = (Player.PlayerA - MyGame.FieldOfView / 2.0f) +
				                  (ix / (float) MyGame.Width) * MyGame.FieldOfView;

				float fDistanceToWall = 0.0f;
				bool bHitWall = false;

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
						Tile t = GetTileAtPosition(nTestX, nTestY);
						if (t.Type == MyGame.TileType.Empty) continue;
						if (!_visibleTiles.Contains(t))
						{
							t.Visible = true;
							t.LastCalculatedDistanceToPlayer = fDistanceToWall;
							_visibleTiles.Add(t);
						}

						bHitWall = true;
					}
				}
			}

			List<Tile> sortedList = _visibleTiles.OrderByDescending(t=>t.LastCalculatedDistanceToPlayer).ToList();
			foreach (Tile tile in sortedList)
			{
				RenderTile(canvas, minimap, tile);
			}
		}

		public Tile GetTileAtPosition(int x, int y)
		{
			return _tiles[x, y];
		}

		private static void RenderTile(EasyDraw canvas, Minimap minimap, Tile t)
		{
			minimap.DebugNoStroke();

			float tileCenterX = t.X + 0.5f;
			float tileCenterY = t.Y + 0.5f;
			minimap.DebugFill(0, 255, 0);
			minimap.DebugCircle(tileCenterX, tileCenterY, 2);

			// Console.WriteLine("--------");
			//Loop through sides of the tile
			for (int a = 0; a < 4; a++)
			{
				float temp = Mathf.PI * a / 2.0f;
				int x = (int)Mathf.Cos(temp);
				int y = (int)Mathf.Sin(temp);

				// Console.WriteLine(x + ", " + y);

				Vector2 sideLocation = new Vector2(tileCenterX + x/2.0f, tileCenterY + y/2.0f);
				minimap.DebugFill(255, 0, 0);
				minimap.DebugCircle(sideLocation.x, sideLocation.y, 2);
				Vector2 sideNormal = new Vector2(x, y);
				// Console.WriteLine(sideNormal.ToString());
				//TODO: Exit if side is facing away from the player

				minimap.DebugFill(0, 0, 255);
				Vector2 p1 = new Vector2(sideLocation.x - sideNormal.y/2.0f, sideLocation.y - sideNormal.x/2.0f);
				minimap.DebugCircle(p1.x, p1.y, 2);
				minimap.DebugStroke(0, 0, 0);
				minimap.DebugStrokeWeight(1);
				minimap.DebugLine(Player.PlayerX, Player.PlayerY, p1.x, p1.y);


				minimap.DebugFill(0, 0, 255);
				Vector2 p2 = new Vector2(sideLocation.x + sideNormal.y/2.0f, sideLocation.y + sideNormal.x/2.0f);
				minimap.DebugCircle(p2.x, p2.y, 2);
				minimap.DebugStroke(0, 0, 0);
				minimap.DebugStrokeWeight(1);
				minimap.DebugLine(Player.PlayerX, Player.PlayerY, p2.x, p2.y);

				minimap.DebugStroke(255, 0, 0);
				Vector2 playerHeading = Vector2.FromAngle(-Player.PlayerA + Mathf.PI/2.0f).Mult(500);
				minimap.DebugLine(Player.PlayerX, Player.PlayerY, playerHeading.x, playerHeading.y);


				Vector2 player = new Vector2(Player.PlayerX, Player.PlayerY);
				float angle1 = Vector2.AngleBetween(playerHeading, p1);
				int ix1 = Mathf.Round(MyGame.Width * ((angle1 - (Player.PlayerA-MyGame.FieldOfView / 2.0f)) / MyGame.FieldOfView));
				float fDistanceToWall1 = Vector2.Dist(player, p1);
				float fCeiling1 = MyGame.Height / 2.0f - MyGame.Height / fDistanceToWall1;
				float fFloor1 = MyGame.Height - fCeiling1;

				float angle2 = Vector2.AngleBetween(playerHeading, p2);
				int ix2 = Mathf.Round(MyGame.Width * ((angle2 - (Player.PlayerA-MyGame.FieldOfView / 2.0f)) / MyGame.FieldOfView));
				float fDistanceToWall2 = Vector2.Dist(player, p2);
				float fCeiling2 = MyGame.Height / 2.0f - MyGame.Height / fDistanceToWall2;
				float fFloor2 = MyGame.Height - fCeiling2;

				//Inverse Square(ish) Law:
				const float exp = 1.6f;
				float sq = Mathf.Pow(t.LastCalculatedDistanceToPlayer, exp);
				float wSq = Mathf.Pow(MyGame.ViewDepth, exp);
				int brightness = (int) MyGame.Map(sq, 0, wSq, 255, 0);
				canvas.Fill(brightness);
				//Linear lighting:
				// canvas.Stroke((int) MyGame.Map(fDistanceToWall, 0, MyGame.ViewDepth, 255, 0));
				canvas.Stroke(0);
				canvas.StrokeWeight(3);
				canvas.Quad(ix1, fCeiling1, ix1, fFloor1, ix2, fFloor2, ix2, fCeiling2);
			}
		}
	}
}