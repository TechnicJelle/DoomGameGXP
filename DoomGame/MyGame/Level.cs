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

		private readonly Tile[,] tiles;
		public int tilesColumns { get; }
		public int tilesRows { get; }

		private List<TileWall> visibleTileWalls;

		public Level(int w, int h, string mapContent)
		{
			tilesColumns = w;
			tilesRows = h;
			if (mapContent.Length != tilesColumns * tilesRows) throw new Exception("TilesWidth * TilesHeight is not mapContent.Length");
			tiles = new Tile[tilesColumns, tilesRows];
			for (int col = 0; col < tilesColumns; col++)
			for (int row = 0; row < tilesRows; row++)
			{
				switch (mapContent[row * tilesRows + col])
				{
					case '#':
						tiles[col, row] = new TileWall(col, row, "checkers.png");
						break;
					case '.':
						tiles[col, row] = new Tile(col, row);
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
			tilesColumns = mainLayer.Width;
			tilesRows = mainLayer.Height;

			short[,] tileNumbers = mainLayer.GetTileArray();

			tiles = new Tile[tilesColumns, tilesRows];
			for (int col = 0; col < tilesColumns; col++)
			for (int row = 0; row < tilesRows; row++)
			{
				switch (tileNumbers[row, col])
				{
					case 0:
						tiles[col, row] = new Tile(col, row);
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
				}
			}
		}

		public void Render(EasyDraw canvas, Minimap minimap)
		{
			//Reset all visible tiles
			visibleTileWalls = new List<TileWall>();
			for (int col = 0; col < tilesColumns; col++)
			for (int row = 0; row < tilesRows; row++)
			{
				if (tiles[col, row].GetType() != typeof(TileWall)) continue;
				TileWall tw = (TileWall) tiles[col, row];
				tw.visible = false;
			}

			//Find tiles to render
			//For every x pixel, send out a ray that goes until it has hit a wall or reached the maximum render distance
			for (int px = 0; px < MyGame.WIDTH; px+=10)
			{
				float rayAngle = (Player.playerA - MyGame.FIELD_OF_VIEW / 2.0f) +
				                  (px / (float) MyGame.WIDTH) * MyGame.FIELD_OF_VIEW;

				float distanceToWall = 0.0f;
				bool hitWall = false;

				float eyeX = Mathf.Sin(rayAngle);
				float eyeY = Mathf.Cos(rayAngle);

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
						if (!visibleTileWalls.Contains(tw))
						{
							tw.visible = true;
							tw.lastCalculatedDistanceToPlayer = distanceToWall;
							visibleTileWalls.Add(tw);
						}

						hitWall = true;
					}
				}
			}

			List<TileWall> sortedList = visibleTileWalls.OrderByDescending(tw=>tw.lastCalculatedDistanceToPlayer).ToList();
			foreach (TileWall tw in sortedList)
			{
				RenderTile(canvas, minimap, tw);
			}
		}

		public Tile GetTileAtPosition(int col, int row)
		{
			return tiles[col, row];
		}

		private static void RenderTile(EasyDraw canvas, Minimap minimap, TileWall tw)
		{
			minimap.DebugNoStroke();

			Vector2 playerHeading = Vector2.FromAngle(-Player.playerA + Mathf.HALF_PI);

			float tileCenterX = tw.col + 0.5f;
			float tileCenterY = tw.row + 0.5f;
			minimap.DebugFill(0, 200, 0);
			minimap.DebugCircle(tileCenterX, tileCenterY, 4);

			//Loop through sides of the tile
			for (int a = 0; a < 4; a++)
			{
				float temp = Mathf.HALF_PI * a;
				int x = (int)Mathf.Cos(temp);
				int y = (int)Mathf.Sin(temp);

				Vector2 sideNormal = new Vector2(x, y);
				//TODO: base threshold on FOV          ↓
				// if (sideNormal.Dot(playerHeading) > 0) //Doesn't render tile sides that are facing away from the player
				// 	continue;
				Vector2 sideLocation = new Vector2(tileCenterX + x/2.0f, tileCenterY + y/2.0f);

				//Tile Side - Red Dot
				minimap.DebugFill(255, 0, 0);
				minimap.DebugCircle(sideLocation.x, sideLocation.y, 2);

				//Tile Side - Right Point
				Vector2 p1 = new Vector2(sideLocation.x - sideNormal.y/2.0f, sideLocation.y - sideNormal.x/2.0f);
				//Minimap: Blue Dot
				minimap.DebugFill(0, 0, 255);
				minimap.DebugCircle(p1.x, p1.y, 2);
				(int ix1, float distToWall1) = WorldToScreen(minimap, p1, playerHeading);
				float fCeiling1 = MyGame.HEIGHT / 2.0f - MyGame.HEIGHT / distToWall1;
				float fFloor1 = MyGame.HEIGHT - fCeiling1;

				//Tile Side - Left Point
				Vector2 p2 = new Vector2(sideLocation.x + sideNormal.y/2.0f, sideLocation.y + sideNormal.x/2.0f);
				//Minimap: Blue Dot
				minimap.DebugFill(0, 0, 255);
				minimap.DebugCircle(p2.x, p2.y, 2);
				(int ix2, float distToWall2) = WorldToScreen(minimap, p2, playerHeading);
				float fCeiling2 = MyGame.HEIGHT / 2.0f - MyGame.HEIGHT / distToWall2;
				float fFloor2 = MyGame.HEIGHT - fCeiling2;

				//Drawing the side
				//Inverse Square(ish) Law:
				const float exp = 1.6f;
				float sq = Mathf.Pow((distToWall1 + distToWall2) / 2.0f, exp);
				float wSq = Mathf.Pow(Player.VIEW_DEPTH, exp);
				int brightness = Mathf.Round(Mathf.Clamp(Mathf.Map(sq, 0, wSq, 255, 0), 0, 255));

				//Linear lighting:
				// canvas.Stroke((int) MyGame.Map(t.LastCalculatedDistanceToPlayer, 0, MyGame.ViewDepth, 255, 0));
				canvas.Fill(brightness);
				canvas.Stroke(0);
				canvas.StrokeWeight(2);
				canvas.Quad(ix1, fCeiling1, ix1, fFloor1, ix2, fFloor2, ix2, fCeiling2);
				//TODO: Stop needing to draw the Quad and probably completely get rid of canvas

				//TODO: This makes it so each Wall only gets one rendered face. Needs to be updated when TileWall.Render(corners) function becomes a thing
				tw.SetCorners(new[] {ix1, fCeiling1, ix2, fCeiling2, ix2, fFloor2, ix1, fFloor1});
				// tw.Render(canvas.game._glContext);
			}
		}

		private static (int, float) WorldToScreen(Minimap minimap, Vector2 p, Vector2 playerHeading)
		{
			minimap.DebugStroke(0);
			minimap.DebugStrokeWeight(1f);

			minimap.DebugLine(Player.position.x, Player.position.y, p.x, p.y);

			Vector2 pp = Vector2.Sub(p, Player.position);
			float distToWall = Vector2.Dist(Player.position, p);
			// minimap.DebugLine(Player.position.x, Player.position.y, Player.position.x + pp.x, Player.position.y + pp.y);
			float angle = Vector2.AngleBetween2(playerHeading, pp);
			if (angle > Mathf.PI)
				angle -= Mathf.TWO_PI; //Thanks https://github.com/EV4gamer

			int ix = Mathf.Round((MyGame.WIDTH / 2.0f) + angle * (MyGame.WIDTH / MyGame.FIELD_OF_VIEW)); //Thanks https://github.com/StevenClifford!
			return (ix, distToWall);
		}
	}
}