using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class MyGame : Game
	{
		public const int WIDTH = 1280;
		public const int HEIGHT = 720;

		public const float FIELD_OF_VIEW = Mathf.PI / 3.0f;

		public static Level level;

		private const bool USE_TILED = true;

		public const bool DEBUG_MODE = false;

		private MyGame(bool pixelArt) : base(WIDTH, HEIGHT, true, pPixelArt: pixelArt)
		{
			//Player
			Player player = new Player(1.5f, 1.5f, Mathf.HALF_PI);
			AddChild(player);

			//Render Background
			EasyDraw background = new EasyDraw(WIDTH, HEIGHT, false);
			for (int iy = 0; iy < HEIGHT; iy++)
			{
				if (iy < HEIGHT / 2)
				{
					// Ceiling
					float fac = Mathf.Map(iy, 0, HEIGHT, 1.0f, 0.4f);
					background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
				}
				else
				{
					//Floor
					background.Stroke((int) Mathf.Map(iy, 0, HEIGHT, 0, 64));
				}

				background.Line(0, iy, WIDTH, iy);
			}
			AddChild(background);

			//Level
			if (USE_TILED)
			{
				level = new Level("Level01.tmx");
			}
			else
			{
				string map = "";
				map += "################";
				map += "#.#............#";
				map += "#.#......#######";
				map += "#.###..........#";
				map += "#..............#";
				map += "#..............#";
				map += "#...#..........#";
				map += "#...#..........#";
				map += "#..............#";
				map += "#........#.....#";
				map += "#........#.....#";
				map += "#..............#";
				map += "#..............#";
				map += "#...###........#";
				map += "#..............#";
				map += "################";
				level = new Level(16, 16, map);
			}

			//Minimap
			Minimap.Setup(0, 0, 200, 200);

			Console.WriteLine("MyGame initialized");
		}

		// For every game object, Update is called every frame, by the engine:
		private void Update()
		{
			if (Input.GetKeyDown(Key.T))
			{
				foreach (UVOffsetSprite uvOffsetSprite in game.FindObjectsOfType<UVOffsetSprite>())
				{
					game.RemoveChild(uvOffsetSprite);
				}
				level.LoadTiledFile("Level02.tmx");
				Minimap.UpdateLevel();
			}

			if(DEBUG_MODE)
				Minimap.ClearDebug();

			Player.MoveInput();

			//First we get all tiles that are on screen
			List<TileWall> onscreenTileWalls = level.FindOnscreenTileWalls();

			//Then we loop through those tiles and see which sides are actually visible
			List<WallSide> visibleSides = new List<WallSide>();
			foreach (TileWall tileWall in onscreenTileWalls)
			{
				visibleSides.AddRange(tileWall.FindVisibleSides());
			}

			//We sort the sides by their distance to the player, to make sure they're rendered in the correct order
			List<WallSide> sorted = visibleSides.OrderByDescending(side => side.distToPlayer).ToList();

			//Then we render all those visible sides
			foreach (WallSide side in sorted)
			{
				side.Refresh();
			}

			Minimap.ReOverlay();
		}

		public static (int, float) WorldToScreen(Vector2 p)
		{
			// Minimap.DebugStroke(0);
			// Minimap.DebugStrokeWeight(1f);

			// Minimap.DebugLine(Player.position.x, Player.position.y, p.x, p.y);

			Vector2 pp = Vector2.Sub(p, Player.position);
			float dist = Vector2.Dist(Player.position, p);
			// Minimap.DebugLine(Player.position.x, Player.position.y, Player.position.x + pp.x, Player.position.y + pp.y);
			float angle = Vector2.AngleBetween2(Player.heading, pp);
			if (angle > Mathf.PI)
				angle -= Mathf.TWO_PI; //Thanks https://github.com/EV4gamer

			int ix = Mathf.Round((MyGame.WIDTH / 2.0f) + angle * (MyGame.WIDTH / MyGame.FIELD_OF_VIEW)); //Thanks https://github.com/StevenClifford!
			return (ix, dist);
		}

		private static void Main() // Main() is the first method that's called when the program is run
		{
			new MyGame(false).Start(); // Create a "MyGame" and start it
		}
	}
}