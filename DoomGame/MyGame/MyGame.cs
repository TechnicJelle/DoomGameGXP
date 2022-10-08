using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.GXPEngine;
using GXPEngine.GXPEngine.AddOns;
using GXPEngine.GXPEngine.Core;
using GXPEngine.GXPEngine.Utils;

#pragma warning disable CS0162

namespace GXPEngine.MyGame;

public class MyGame : Game
{
	public static int StaticWidth;
	public static int StaticHeight;
	private readonly EasyDraw _mainMenu;

	public static float FieldOfView;

	private static Level[] _levels;
	public static Level CurrentLevel { get; private set; }
	private int _currentLevelIndex;

	private const int MILLIS_FOR_TITLE = 3000;
	private readonly EasyDraw _title;
	private int _millisAtTitleShow;

	private const bool USE_TILED = true;

	private bool _inGame;
	private bool _gameJustEnded;

	private MyGame(int width, int height, bool fullScreen, int realWidth, int realHeight, bool pixelArt)
		: base(width, height, fullScreen, true, realWidth, realHeight, pixelArt)
	{
		StaticWidth = width;
		StaticHeight = height;

		FieldOfView = Settings.FieldOfViewDegrees * Mathf.PI / 180.0f;

		//Render Background
		EasyDraw background = new(StaticWidth, StaticHeight, false);
		for (int iy = 0; iy < StaticHeight; iy++)
		{
			if (iy < StaticHeight / 2)
			{
				// Ceiling
				float fac = Mathf.Clamp(Mathf.Map(iy, 0, StaticHeight, 1.0f, -3), 0.0f, 1.0f);
				background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
			}
			else
			{
				//Floor
				background.Stroke((int) Mathf.Clamp(Mathf.Map(iy, 0, StaticHeight, -300, 64), 0, 255));
			}

			background.Line(0, iy, StaticWidth, iy);
		}
		AddChild(background);

		//Render main menu
		_mainMenu = new EasyDraw(StaticWidth, StaticHeight, false);
		_mainMenu.TextAlign(CenterMode.Center, CenterMode.Center);
		_mainMenu.TextSize(StaticHeight * 0.09f);
		_mainMenu.Text("TechnicJelle's DoomGame", StaticWidth * 0.5f, StaticHeight * 0.2f);

		AddChild(_mainMenu);

		//Set up the title shower
		_title = new EasyDraw(StaticWidth, StaticHeight, false);
		_title.TextAlign(CenterMode.Center, CenterMode.Center);
		_title.TextSize(StaticHeight * 0.1f);

		//Prepare the music
		Sounds.LoadAllSounds();
		Sounds.Music.Play();

		Console.WriteLine("MyGame initialized");
	}

	private void StartGame()
	{
		RemoveChild(_mainMenu);

		//Minimap
		if(Settings.Minimap)
			Minimap.Setup(0, 0, 200, 200);

		//Level
		if (USE_TILED)
		{
			_levels = new[]
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
			_levels = new []
			{
				new Level(16, 16, map),
			};
		}

		_currentLevelIndex = 0;
		SwitchLevel(_currentLevelIndex);
		if(Settings.Minimap)
			Minimap.DrawCurrentLevel();

		_inGame = true;
	}

	// For every game object, Update is called every frame by the engine:
	private void Update()
	{
		if (!_inGame)
		{
			if (_gameJustEnded)
			{
				AddChild(_mainMenu);
				_title.alpha = 0;
				_gameJustEnded = false;
				RemoveAllWarpedSprites();
				if (Settings.Minimap)
					Minimap.ClearAll();
			}

			bool overButton = Input.mouseX > StaticWidth * 0.25 && Input.mouseX < StaticWidth * 0.75f &&
			                  Input.mouseY > StaticHeight * 0.7f && Input.mouseY < StaticHeight * 0.9f;
			_mainMenu.Fill(overButton ? 200 : 100);
			_mainMenu.StrokeWeight(5);
			_mainMenu.Stroke(164);
			_mainMenu.Rect(StaticWidth * 0.5f, StaticHeight * 0.8f, StaticWidth * 0.5f, StaticHeight * 0.2f);

			_mainMenu.Fill(0);
			_mainMenu.TextSize(StaticHeight * 0.16f);
			_mainMenu.Text("Start!", StaticWidth * 0.5f, StaticHeight * 0.81f);

			if (Input.AnyKeyDown() || overButton && Input.GetMouseButtonDown(0))
			{
				StartGame();
				Sounds.ButtonClick.Play();
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

			RemoveChild(_title);

			if (Settings.Minimap)
				Minimap.ClearDebug();

			if (Settings.Minimap)
				Minimap.ClearEnemies();

			CurrentLevel.Player.MoveInput(this);
			CurrentLevel.MoveEnemies();

			//Make the render pool
			List<Renderable> visibleRenderables = new();

			//First we get all tiles that are on screen
			List<TileWall> onscreenTileWalls = CurrentLevel.FindOnscreenTileWalls();

			//Then we loop through those tiles and see which sides are actually visible
			foreach (TileWall tileWall in onscreenTileWalls)
			{
				//And we add those visible sides to the rendering pool
				visibleRenderables.AddRange(tileWall.FindVisibleSides());
			}

			//Enemies need to be rendered too, of course
			visibleRenderables.AddRange(CurrentLevel.FindVisibleEnemies());

			//We sort the renderables by their distance to the player, to make sure they're rendered in the correct order
			List<Renderable> sorted = visibleRenderables.OrderByDescending(renderable => renderable.DistToPlayer).ToList();

			//Then we refresh all those renderables
			foreach (Renderable renderable in sorted)
			{
				renderable.RefreshVisuals();
			}

			_title.alpha = Mathf.Clamp(Mathf.Map(Time.time - _millisAtTitleShow, 0, MILLIS_FOR_TITLE, 2, 0), 0, 1);
			if (Time.time - _millisAtTitleShow > MILLIS_FOR_TITLE)
				_title.alpha = 0;
			AddChild(_title);

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
		_title.ClearTransparent();
		_title.Text(titleText, StaticWidth * 0.5f, StaticHeight * 0.5f);
		_title.alpha = 1;
		_millisAtTitleShow = Time.time;
	}

	public static (int, float) WorldToScreen(Vector2 p)
	{
		// Minimap.DebugStroke(0);
		// Minimap.DebugStrokeWeight(1f);

		// Minimap.DebugLine(Player.position.x, Player.position.y, p.x, p.y);

		Vector2 pp = p - CurrentLevel.Player.Position;
		float dist = Vector2.Dist(CurrentLevel.Player.Position, p);
		// Minimap.DebugLine(Player.position.x, Player.position.y, Player.position.x + pp.x, Player.position.y + pp.y);
		float angle = Angle.Difference(CurrentLevel.Player.Heading.Heading(), pp.Heading());
		if (angle > Mathf.PI)
			angle -= Mathf.TWO_PI; //Thanks https://github.com/EV4gamer

		int ix = Mathf.Round((StaticWidth / 2.0f) + angle * (StaticWidth / FieldOfView)); //Thanks https://github.com/StevenClifford!
		return (ix, dist);
	}

	public void NextLevel()
	{
		Sounds.Elevator.Play();
		_currentLevelIndex++;
		if (_currentLevelIndex >= _levels.Length)
		{
			_inGame = false;
			_gameJustEnded = true;
		}
		else
			SwitchLevel(_currentLevelIndex);
	}

	private void SwitchLevel(int index)
	{
		RemoveAllWarpedSprites();
		_currentLevelIndex = index;
		CurrentLevel = _levels[_currentLevelIndex];
		CurrentLevel.SetVisibility(true);

		if (Settings.Minimap)
		{
			Minimap.UpdateLevel();
			Minimap.UpdatePlayer();
		}

		SetTitle(CurrentLevel.Title);
	}

	private static void Main() // Main() is the first method that's called when the program is run
	{
		Settings.Load();
		new MyGame(Settings.Width, Settings.Height, Settings.FullScreen, Settings.ScreenResolutionX, Settings.ScreenResolutionY, !Settings.AntiAliasing)
			.Start(); // Create a "MyGame" and start it
	}
}
