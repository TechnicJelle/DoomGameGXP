using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;
using TiledMapParser;

namespace GXPEngine.MyGame
{
	public class Level
	{
		private const float RENDER_DISTANCE = 32.0f;

		private readonly Tile[,] _tiles;
		public int TilesColumns { get; }
		public int TilesRows { get; }

		private List<Tile> _visibleTiles;

		public Level(int w, int h, string mapContent)
		{
			TilesColumns = w;
			TilesRows = h;
			if (mapContent.Length != TilesColumns * TilesRows) throw new Exception("TilesWidth * TilesHeight is not mapContent.Length");
			_tiles = new Tile[TilesColumns, TilesRows];
			for (int col = 0; col < TilesColumns; col++)
			for (int row = 0; row < TilesRows; row++)
			{
				switch (mapContent[row * TilesRows + col])
				{
					case '#':
						_tiles[col, row] = new Tile(MyGame.TileType.Wall, col, row, "checkers.png");
						break;
					case '.':
						_tiles[col, row] = new Tile(MyGame.TileType.Empty, col, row);
						break;
				}
			}
		}

		public Level(string tiledFile)
		{
			Map levelData = MapParser.ReadMap(tiledFile);
			if (levelData.Layers == null || levelData.Layers.Length <= 0)
				throw new Exception("Tile file " + tiledFile + " does not contain a layer!");
			Layer mainLayer = levelData.Layers[0];
			TilesColumns = mainLayer.Width;
			TilesRows = mainLayer.Height;

			short[,] tileNumbers = mainLayer.GetTileArray();

			_tiles = new Tile[TilesColumns, TilesRows];
			for (int col = 0; col < TilesColumns; col++)
			for (int row = 0; row < TilesRows; row++)
			{
				switch (tileNumbers[row, col])
				{
					case 0:
						_tiles[col, row] = new Tile(MyGame.TileType.Empty, col, row);
						break;
					case 1:
						_tiles[col, row] = new Tile(MyGame.TileType.Wall, col, row, "square.png");
						break;
					case 2:
						_tiles[col, row] = new Tile(MyGame.TileType.Wall, col, row, "colors.png");
						break;
					case 3:
						_tiles[col, row] = new Tile(MyGame.TileType.Wall, col, row, "checkers.png");
						break;
				}
			}
		}

		public void Render(EasyDraw canvas, Minimap minimap)
		{
			//Reset all visible tiles
			_visibleTiles = new List<Tile>();
			for (int col = 0; col < TilesColumns; col++)
			for (int row = 0; row < TilesRows; row++)
			{
				_tiles[col, row].Visible = false;
			}

			//Find tiles to render
			//For every x pixel, send out a ray that goes until it has hit a wall or reached the maximum render distance
			for (int px = 0; px < MyGame.Width; px+=10)
			{
				float fRayAngle = (Player.PlayerA - MyGame.FieldOfView / 2.0f) +
				                  (px / (float) MyGame.Width) * MyGame.FieldOfView;

				float fDistanceToWall = 0.0f;
				bool bHitWall = false;

				float fEyeX = Mathf.Sin(fRayAngle);
				float fEyeY = Mathf.Cos(fRayAngle);

				while (!bHitWall && fDistanceToWall < RENDER_DISTANCE)
				{
					fDistanceToWall += 0.1f;

					int nTestX = (int) (Player.Position.x + fEyeX * fDistanceToWall);
					int nTestY = (int) (Player.Position.y + fEyeY * fDistanceToWall);

					if (nTestX < 0 || nTestX >= TilesColumns | nTestY < 0 || nTestY >= TilesRows)
					{
						//Ray has gone out of map bounds
						bHitWall = true;
						fDistanceToWall = RENDER_DISTANCE;
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

		public Tile GetTileAtPosition(int col, int row)
		{
			return _tiles[col, row];
		}

		private static void RenderTile(EasyDraw canvas, Minimap minimap, Tile t)
		{
			minimap.DebugNoStroke();

			Vector2 playerHeading = Vector2.FromAngle(-Player.PlayerA + Mathf.PI/2.0f);

			float tileCenterX = t.Col + 0.5f;
			float tileCenterY = t.Row + 0.5f;
			minimap.DebugFill(0, 200, 0);
			minimap.DebugCircle(tileCenterX, tileCenterY, 4);

			//Loop through sides of the tile
			for (int a = 0; a < 4; a++)
			{
				float temp = Mathf.PI * a / 2.0f;
				int x = (int)Mathf.Cos(temp);
				int y = (int)Mathf.Sin(temp);

				Vector2 sideNormal = new Vector2(x, y);
				if (sideNormal.Dot(playerHeading) > 0) //Doesn't render tile sides that are facing away from the player
					continue;
				Vector2 sideLocation = new Vector2(tileCenterX + x/2.0f, tileCenterY + y/2.0f);

				//Tile Side - Red Dot
				minimap.DebugFill(255, 0, 0);
				minimap.DebugCircle(sideLocation.x, sideLocation.y, 2);

				//Tile Side - Right Point
				Vector2 p1 = new Vector2(sideLocation.x - sideNormal.y/2.0f, sideLocation.y - sideNormal.x/2.0f);
				//Minimap: Line
				minimap.DebugStroke(0);
				minimap.DebugStrokeWeight(1);
				minimap.DebugLine(Player.Position.x, Player.Position.y, p1.x, p1.y);
				//Minimap: Blue Dot
				minimap.DebugFill(0, 0, 255);
				minimap.DebugCircle(p1.x, p1.y, 2);

				float angle1 = Vector2.AngleBetween(playerHeading, p1);
				int ix1 = Mathf.Round(MyGame.Width * ((angle1 - ((Player.PlayerA % (2.0f*Mathf.PI))-MyGame.FieldOfView / 2.0f)) / MyGame.FieldOfView)); //TODO: Fix
				float fDistanceToWall1 = Vector2.Dist(Player.Position, p1);
				float fCeiling1 = MyGame.Height / 2.0f - MyGame.Height / fDistanceToWall1;
				float fFloor1 = MyGame.Height - fCeiling1;

				//Tile Side - Left Point
				Vector2 p2 = new Vector2(sideLocation.x + sideNormal.y/2.0f, sideLocation.y + sideNormal.x/2.0f);
				//Minimap: Line
				minimap.DebugStroke(0);
				minimap.DebugStrokeWeight(1);
				minimap.DebugLine(Player.Position.x, Player.Position.y, p2.x, p2.y);
				//Minimap: Blue Dot
				minimap.DebugFill(0, 0, 255);
				minimap.DebugCircle(p2.x, p2.y, 2);

				float angle2 = Vector2.AngleBetween(playerHeading, p2);
				int ix2 = Mathf.Round(MyGame.Width * ((angle2 - (Player.PlayerA % (2.0f*Mathf.PI)-MyGame.FieldOfView / 2.0f)) / MyGame.FieldOfView)); //TODO: Fix
				float fDistanceToWall2 = Vector2.Dist(Player.Position, p2);
				float fCeiling2 = MyGame.Height / 2.0f - MyGame.Height / fDistanceToWall2;
				float fFloor2 = MyGame.Height - fCeiling2;

				//Drawing the side
				//Inverse Square(ish) Law:
				const float exp = 1.6f;
				float sq = Mathf.Pow(t.LastCalculatedDistanceToPlayer, exp);
				float wSq = Mathf.Pow(Player.ViewDepth, exp);
				int brightness = Mathf.Round(Mathf.Clamp(MyGame.Map(sq, 0, wSq, 255, 0), 0, 255));

				//Linear lighting:
				// canvas.Stroke((int) MyGame.Map(t.LastCalculatedDistanceToPlayer, 0, MyGame.ViewDepth, 255, 0));
				canvas.Fill(brightness);
				canvas.Stroke(0);
				canvas.StrokeWeight(2);
				canvas.Quad(ix1, fCeiling1, ix1, fFloor1, ix2, fFloor2, ix2, fCeiling2);
				// t.RenderSide(Game.main._glContext, new[] {ix1, fCeiling1, ix1, fFloor1, ix2, fFloor2, ix2, fCeiling2});
			}

			//Player Heading
			playerHeading.Mult(23).Add(Player.Position);
			minimap.DebugStroke(255, 0, 0);
			minimap.DebugLine(Player.Position.x, Player.Position.y, playerHeading.x, playerHeading.y);
		}
	}
}