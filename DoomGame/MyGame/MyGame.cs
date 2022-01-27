using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;
#pragma warning disable CS0162

namespace GXPEngine.MyGame
{
	public class MyGame : Game
	{
		public const int WIDTH = 1280;
		public const int HEIGHT = 720;
		private readonly EasyDraw mainMenu;

		public const float FIELD_OF_VIEW = Mathf.PI / 2.5f;

		private static Level[] levels;
		public static Level currentLevel;
		private int currentLevelIndex;

		private const bool USE_TILED = true;

		public const bool DEBUG_MODE = false;

		private bool inGame;
		private bool gameJustEnded;

		private MyGame(bool pixelArt) : base(WIDTH, HEIGHT, false, pPixelArt: pixelArt)
		{
			//Render Background
			EasyDraw background = new EasyDraw(WIDTH, HEIGHT, false);
			for (int iy = 0; iy < HEIGHT; iy++)
			{
				if (iy < HEIGHT / 2)
				{
					// Ceiling
					float fac = Mathf.Clamp(Mathf.Map(iy, 0, HEIGHT, 1.0f, -3), 0.0f, 1.0f);
					background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
				}
				else
				{
					//Floor
					background.Stroke((int) Mathf.Clamp(Mathf.Map(iy, 0, HEIGHT, -300, 64), 0, 255));
				}

				background.Line(0, iy, WIDTH, iy);
			}
			AddChild(background);

			mainMenu = new EasyDraw(WIDTH, HEIGHT, false);
			mainMenu.TextAlign(CenterMode.Center, CenterMode.Center);
			mainMenu.TextSize(HEIGHT * 0.1f);
			mainMenu.Text("TechnicJelle's DoomGame", WIDTH * 0.5f, HEIGHT * 0.2f);

			AddChild(mainMenu);

			Console.WriteLine("MyGame initialized");
		}

		private void StartGame()
		{
			RemoveChild(mainMenu);

			//Minimap
			Minimap.Setup(0, 0, 200, 200);

			//Level
			if (USE_TILED)
			{
				levels = new[]
				{
					new Level("Level01.tmx"),
					new Level("Level02.tmx"),
					new Level("Level03.tmx"),
				};
			}
			else
			{
				string map = "";
				map += "################";
				map += "#.#...........E#";
				map += "#.#......#######";
				map += "#.###..........#";
				map += "#..............#";
				map += "#..............#";
				map += "#...#..........#";
				map += "#...#..........#";
				map += "#......P.......#";
				map += "#........#.....#";
				map += "#........#.....#";
				map += "#..............#";
				map += "#..............#";
				map += "#...###........#";
				map += "#..............#";
				map += "################";
				levels = new []
				{
					new Level(16, 16, map),
				};
			}

			currentLevelIndex = 0;
			SwitchLevel(currentLevelIndex);
			Minimap.DrawCurrentLevel();

			inGame = true;
		}

		// For every game object, Update is called every frame by the engine:
		private void Update()
		{
			if (!inGame)
			{
				if (gameJustEnded)
				{
					AddChild(mainMenu);
					gameJustEnded = false;
					RemoveAllWarpedSprites();
					Minimap.ClearAll();
				}

				bool overButton = Input.mouseX > WIDTH * 0.25 && Input.mouseX < WIDTH * 0.75f &&
				                  Input.mouseY > HEIGHT * 0.7f && Input.mouseY < HEIGHT * 0.9f;
				mainMenu.Fill(overButton ? 200 : 100);
				mainMenu.StrokeWeight(5);
				mainMenu.Stroke(164);
				mainMenu.Rect(WIDTH * 0.5f, HEIGHT * 0.8f, WIDTH * 0.5f, HEIGHT * 0.2f);

				mainMenu.Fill(0);
				mainMenu.TextSize(HEIGHT * 0.16f);
				mainMenu.Text("Start!", WIDTH * 0.5f, HEIGHT * 0.81f);

				if (overButton && Input.GetMouseButtonDown(0))
				{
					StartGame();
				}
			}
			else
			{
				if (Input.GetKeyDown(Key.T))
				{
					NextLevel();
				} else if (Input.GetKeyDown(Key.ONE))
				{
					StartGame();
					SwitchLevel(0);
				}
				else if (Input.GetKeyDown(Key.TWO))
				{
					StartGame();
					SwitchLevel(1);
				}
				else if (Input.GetKeyDown(Key.THREE))
				{
					StartGame();
					SwitchLevel(2);
				}
				else if (Input.GetKeyDown(Key.FOUR))
				{
					StartGame();
					SwitchLevel(3);
				}
				else if (Input.GetKeyDown(Key.FIVE))
				{
					StartGame();
					SwitchLevel(4);
				}
				else if (Input.GetKeyDown(Key.SIX))
				{
					StartGame();
					SwitchLevel(5);
				}
				else if (Input.GetKeyDown(Key.SEVEN))
				{
					StartGame();
					SwitchLevel(6);
				}
				else if (Input.GetKeyDown(Key.EIGHT))
				{
					StartGame();
					SwitchLevel(7);
				}
				else if (Input.GetKeyDown(Key.NINE))
				{
					StartGame();
					SwitchLevel(8);
				}
				else if (Input.GetKeyDown(Key.ZERO))
				{
					StartGame();
					SwitchLevel(9);
				}
				else if (Input.GetKeyDown(Key.PLUS))
				{
					NextLevel();
				}

				Minimap.ClearDebug();

				Minimap.ClearEnemies();

				currentLevel.player.MoveInput(this);
				currentLevel.MoveEnemies();

				//Make the render pool
				List<Renderable> visibleRenderables = new List<Renderable>();

				//First we get all tiles that are on screen
				List<TileWall> onscreenTileWalls = currentLevel.FindOnscreenTileWalls();

				//Then we loop through those tiles and see which sides are actually visible
				foreach (TileWall tileWall in onscreenTileWalls)
				{
					//And we add those visible sides to the rendering pool
					visibleRenderables.AddRange(tileWall.FindVisibleSides());
				}

				//Enemies need to be rendered too, of course
				visibleRenderables.AddRange(currentLevel.FindVisibleEnemies());

				//We sort the renderables by their distance to the player, to make sure they're rendered in the correct order
				List<Renderable> sorted = visibleRenderables.OrderByDescending(renderable => renderable.distToPlayer).ToList();

				//Then we refresh all those renderables
				foreach (Renderable renderable in sorted)
				{
					renderable.RefreshVisuals();
				}

				Minimap.ReOverlay();
			}

			if(Input.GetKeyDown(Key.P))
				Console.WriteLine(GetDiagnostics());
		}

		private void RemoveAllWarpedSprites()
		{
			foreach (UVOffsetSprite uvOffsetSprite in game.FindObjectsOfType<UVOffsetSprite>())
			{
				game.RemoveChild(uvOffsetSprite);
			}
		}

		public static (int, float) WorldToScreen(Vector2 p)
		{
			// Minimap.DebugStroke(0);
			// Minimap.DebugStrokeWeight(1f);

			// Minimap.DebugLine(Player.position.x, Player.position.y, p.x, p.y);

			Vector2 pp = Vector2.Sub(p, currentLevel.player.position);
			float dist = Vector2.Dist(currentLevel.player.position, p);
			// Minimap.DebugLine(Player.position.x, Player.position.y, Player.position.x + pp.x, Player.position.y + pp.y);
			float angle = Vector2.AngleBetween2(currentLevel.player.heading, pp);
			if (angle > Mathf.PI)
				angle -= Mathf.TWO_PI; //Thanks https://github.com/EV4gamer

			int ix = Mathf.Round((WIDTH / 2.0f) + angle * (WIDTH / FIELD_OF_VIEW)); //Thanks https://github.com/StevenClifford!
			return (ix, dist);
		}

		public void NextLevel()
		{
			currentLevelIndex++;
			if (currentLevelIndex >= levels.Length)
			{
				inGame = false;
				gameJustEnded = true;
			}
			else
				SwitchLevel(currentLevelIndex);
		}

		private void SwitchLevel(int index)
		{
			RemoveAllWarpedSprites();
			currentLevel = levels[index];
			currentLevel.SetVisibility(true);
			Minimap.UpdateLevel();
			Minimap.UpdatePlayer();
		}

		private static void Main() // Main() is the first method that's called when the program is run
		{
			new MyGame(true).Start(); // Create a "MyGame" and start it
		}
	}
}