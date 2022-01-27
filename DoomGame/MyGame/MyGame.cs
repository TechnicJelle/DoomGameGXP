using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;
#pragma warning disable CS0162

namespace GXPEngine.MyGame
{
	public class MyGame : Game
	{
		public static int staticWidth;
		public static int staticHeight;
		private readonly EasyDraw mainMenu;

		public static float fieldOfView;

		private static Level[] levels;
		public static Level currentLevel { get; private set; }
		private int currentLevelIndex;

		private const int MILLIS_FOR_TITLE = 3000;
		private readonly EasyDraw title;
		private int millisAtTitleShow;

		private const bool USE_TILED = true;

		private bool inGame;
		private bool gameJustEnded;

		private MyGame(int width, int height, bool fullScreen, int realWidth, int realHeight, bool pixelArt)
			: base(width, height, fullScreen, true, realWidth, realHeight, pixelArt)
		{
			staticWidth = width;
			staticHeight = height;

			fieldOfView = Settings.FieldOfViewDegrees * Mathf.PI / 180.0f;

			//Render Background
			EasyDraw background = new(staticWidth, staticHeight, false);
			for (int iy = 0; iy < staticHeight; iy++)
			{
				if (iy < staticHeight / 2)
				{
					// Ceiling
					float fac = Mathf.Clamp(Mathf.Map(iy, 0, staticHeight, 1.0f, -3), 0.0f, 1.0f);
					background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
				}
				else
				{
					//Floor
					background.Stroke((int) Mathf.Clamp(Mathf.Map(iy, 0, staticHeight, -300, 64), 0, 255));
				}

				background.Line(0, iy, staticWidth, iy);
			}
			AddChild(background);

			//Render main menu
			mainMenu = new EasyDraw(staticWidth, staticHeight, false);
			mainMenu.TextAlign(CenterMode.Center, CenterMode.Center);
			mainMenu.TextSize(staticHeight * 0.09f);
			mainMenu.Text("TechnicJelle's DoomGame", staticWidth * 0.5f, staticHeight * 0.2f);

			AddChild(mainMenu);

			//Set up the title shower
			title = new EasyDraw(staticWidth, staticHeight, false);
			title.TextAlign(CenterMode.Center, CenterMode.Center);
			title.TextSize(staticHeight * 0.1f);

			//Prepare the music
			Sounds.LoadAllSounds();
			Sounds.music.Play();

			Console.WriteLine("MyGame initialized");
		}

		private void StartGame()
		{
			RemoveChild(mainMenu);

			//Minimap
			if(Settings.Minimap)
				Minimap.Setup(0, 0, 200, 200);

			//Level
			if (USE_TILED)
			{
				levels = new[]
				{
					new Level("Level01.tmx", "Floor 3/3"),
					new Level("Level02.tmx", "Floor 2/3"),
					new Level("Level03.tmx", "Floor 1/3"),
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
			if(Settings.Minimap)
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
					title.alpha = 0;
					gameJustEnded = false;
					RemoveAllWarpedSprites();
					if (Settings.Minimap)
						Minimap.ClearAll();
				}

				bool overButton = Input.mouseX > staticWidth * 0.25 && Input.mouseX < staticWidth * 0.75f &&
				                  Input.mouseY > staticHeight * 0.7f && Input.mouseY < staticHeight * 0.9f;
				mainMenu.Fill(overButton ? 200 : 100);
				mainMenu.StrokeWeight(5);
				mainMenu.Stroke(164);
				mainMenu.Rect(staticWidth * 0.5f, staticHeight * 0.8f, staticWidth * 0.5f, staticHeight * 0.2f);

				mainMenu.Fill(0);
				mainMenu.TextSize(staticHeight * 0.16f);
				mainMenu.Text("Start!", staticWidth * 0.5f, staticHeight * 0.81f);

				if (Input.AnyKeyDown() || overButton && Input.GetMouseButtonDown(0))
				{
					StartGame();
					Sounds.buttonClick.Play();
				}
			}
			else
			{
				if (Settings.DebugMode)
				{
					if (Input.GetKeyDown(Key.T))
					{
						NextLevel();
					}
					else if (Input.GetKeyDown(Key.ONE))
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
				}

				RemoveChild(title);

				if (Settings.Minimap)
					Minimap.ClearDebug();

				if (Settings.Minimap)
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

				title.alpha = Mathf.Clamp(Mathf.Map(Time.time - millisAtTitleShow, 0, MILLIS_FOR_TITLE, 2, 0), 0, 1);
				if (Time.time - millisAtTitleShow > MILLIS_FOR_TITLE)
					title.alpha = 0;
				AddChild(title);

				if (Settings.Minimap)
					Minimap.ReOverlay();
			}

			if(Settings.DebugMode && Input.GetKeyDown(Key.P))
				Console.WriteLine(GetDiagnostics());
		}

		private void RemoveAllWarpedSprites()
		{
			foreach (UVOffsetSprite uvOffsetSprite in game.FindObjectsOfType<UVOffsetSprite>())
			{
				game.RemoveChild(uvOffsetSprite);
			}
		}

		private void SetTitle(string titleText)
		{
			title.ClearTransparent();
			title.Text(titleText, staticWidth * 0.5f, staticHeight * 0.5f);
			title.alpha = 1;
			millisAtTitleShow = Time.time;
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

			int ix = Mathf.Round((staticWidth / 2.0f) + angle * (staticWidth / fieldOfView)); //Thanks https://github.com/StevenClifford!
			return (ix, dist);
		}

		public void NextLevel()
		{
			Sounds.elevator.Play();
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
			currentLevelIndex = index;
			currentLevel = levels[currentLevelIndex];
			currentLevel.SetVisibility(true);

			if (Settings.Minimap)
			{
				Minimap.UpdateLevel();
				Minimap.UpdatePlayer();
			}

			SetTitle(currentLevel.title);
		}

		private static void Main() // Main() is the first method that's called when the program is run
		{
			Settings.Load();
			new MyGame(Settings.Width, Settings.Height, Settings.FullScreen, Settings.ScreenResolutionX, Settings.ScreenResolutionY, !Settings.AntiAliasing)
				.Start(); // Create a "MyGame" and start it
		}
	}
}