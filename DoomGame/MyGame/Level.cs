using System;
using System.Collections.Generic;
using TiledMapParser;

namespace GXPEngine.MyGame
{
	public class Level
	{
		private const float RENDER_DISTANCE = 32.0f;

		private Tile[,] tiles;
		public int tilesColumns { get; private set; }
		public int tilesRows { get; private set; }

		public Level(int w, int h, string mapContent)
		{
			tilesColumns = w;
			tilesRows = h;
			if (mapContent.Length != tilesColumns * tilesRows) throw new Exception("TilesWidth * TilesHeight is not mapContent.Length");
			tiles = new Tile[tilesColumns, tilesRows];
			for (int row = 0; row < tilesRows; row++)
			for (int col = 0; col < tilesColumns; col++)
			{
				switch (mapContent[row * tilesRows + col])
				{
					case '#':
						tiles[col, row] = new TileWall(col, row, "checkers.png");
						break;
					case '.':
						tiles[col, row] = new Tile();
						break;
				}
			}
		}

		public Level(string tiledFile)
		{
			LoadTiledFile(tiledFile);
		}

		public void LoadTiledFile(string tiledFile)
		{
			Map levelData = MapParser.ReadMap(tiledFile);
			if (levelData.Layers == null || levelData.Layers.Length <= 0)
				throw new Exception("Tile file " + tiledFile + " does not contain a layer!");
			Layer mainLayer = levelData.Layers[0];
			tilesColumns = mainLayer.Width;
			tilesRows = mainLayer.Height;

			short[,] tileNumbers = mainLayer.GetTileArray();
			for (int row = 0; row < tilesRows; row++)
			{
				for (int col = 0; col < tilesColumns; col++)
				{
					Console.Write(tileNumbers[col, row]);
				}
				Console.WriteLine("");
			}

			tiles = new Tile[tilesColumns, tilesRows];
			for (int row = 0; row < tilesRows; row++)
			for (int col = 0; col < tilesColumns; col++)
			{
				switch (tileNumbers[col, row])
				{
					case 0:
						tiles[col, row] = new Tile();
						break;
					case 1:
						tiles[col, row] = new TileWall(col, row, "square.png");
						break;
					case 2:
						tiles[col, row] = new TileWall(col, row, "colors.png");
						break;
					case 3:
						tiles[col, row] = new TileWall(col, row, "checkers.png");
						break;
					case 4:
						tiles[col, row] = new TileWall(col, row, "diagonal.png");
						break;
					case 5:
						tiles[col, row] = new TileWall(col, row, "noise.png");
						break;
				}
			}
		}

		public List<TileWall> FindOnscreenTileWalls()
		{
			//Reset all visible tiles
			List<TileWall> onscreenTileWalls = new List<TileWall>();
			for (int col = 0; col < tilesColumns; col++)
			for (int row = 0; row < tilesRows; row++)
			{
				if (tiles[col, row].GetType() != typeof(TileWall)) continue;
				TileWall tw = (TileWall) tiles[col, row];
				tw.SetVisibility(false);
			}

			//Find tiles to render
			//For every x pixel, send out a ray that goes until it has hit a wall or reached the maximum render distance
			for (int px = 0; px < MyGame.WIDTH; px+=10)
			{
				float rayAngle = (Player.angle - MyGame.FIELD_OF_VIEW / 2.0f) +
				                  (px / (float) MyGame.WIDTH) * MyGame.FIELD_OF_VIEW;

				//TODO: Implement DDA here too

				// Vector2 endPoint = Vector2.FromAngle(rayAngle).Mult(RENDER_DISTANCE);
				// Minimap.DebugStroke(0, 100);
				// Minimap.DebugStrokeWeight(1);
				// Minimap.DebugLine(Player.position.x, Player.position.y, Player.position.x + endPoint.x, Player.position.y + endPoint.y);

				float distanceToWall = 0.0f;
				bool hitWall = false;

				float eyeX = Mathf.Cos(rayAngle);
				float eyeY = Mathf.Sin(rayAngle);

				while (!hitWall && distanceToWall < RENDER_DISTANCE)
				{
					distanceToWall += 0.1f;

					int testX = (int) (Player.position.x + eyeX * distanceToWall);
					int testY = (int) (Player.position.y + eyeY * distanceToWall);

					if (testX < 0 || testX >= tilesColumns | testY < 0 || testY >= tilesRows)
					{
						//Ray has gone out of map bounds
						hitWall = true;
						distanceToWall = RENDER_DISTANCE;
					}
					else
					{
						Tile t = GetTileAtPosition(testX, testY);
						if (t.GetType() != typeof(TileWall)) continue;
						TileWall tw = (TileWall) t;
						if (!onscreenTileWalls.Contains(tw))
						{
							onscreenTileWalls.Add(tw);
						}

						hitWall = true;
						Minimap.DebugNoStroke();
						Minimap.DebugFill(0, 255, 0);
						Minimap.DebugCircle(testX + 0.5f, testY + 0.5f, 4);
					}
				}
			}

			return onscreenTileWalls;
		}

		public Tile GetTileAtPosition(int col, int row)
		{
			return tiles[col, row];
		}
	}
}