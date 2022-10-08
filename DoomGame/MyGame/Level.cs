using System;
using System.Collections.Generic;
using GXPEngine.GXPEngine.AddOns;
using GXPEngine.GXPEngine.Core;
using GXPEngine.GXPEngine.Utils;

namespace GXPEngine.MyGame;

public class Level
{
	private const float RENDER_DISTANCE = 4.0f;

	private Tile[,] _tiles;
	public int TilesColumns { get; private set; }
	public int TilesRows { get; private set; }
	private Enemy[] _enemies;

	public Player Player { get; private set; }

	public string Title { get; }

	/// <summary>
	/// Load in a level from a string
	/// </summary>
	/// <param name="w">Width of the level</param>
	/// <param name="h">Height of the level</param>
	/// <param name="mapContent">String with the level contents
	/// <li># is a WallTile</li>
	/// <li>. is an empty Tile</li>
	/// <li>P is a TileStart</li>
	/// <li>E is a TileNext</li>
	/// </param>
	/// <exception cref="Exception">When the level isn't right</exception>
	public Level(int w, int h, string mapContent)
	{
		TilesColumns = w;
		TilesRows = h;
		if (mapContent.Length != TilesColumns * TilesRows) throw new Exception("TilesWidth * TilesHeight != mapContent.Length");
		_tiles = new Tile[TilesColumns, TilesRows];
		Vector2? start = null;
		_enemies = Array.Empty<Enemy>();
		for (int row = 0; row < TilesRows; row++)
		for (int col = 0; col < TilesColumns; col++)
		{
			switch (mapContent[row * TilesRows + col])
			{
				case '#':
					_tiles[col, row] = new TileWall(col, row, "checkers.png");
					break;
				case '.':
					_tiles[col, row] = new Tile();
					break;
				case 'P':
					_tiles[col, row] = new TileStart(col, row);
					start = new Vector2(col + 0.5f, row + 0.5f);
					break;
				case 'E':
					_tiles[col, row] = new TileNext(col, row);
					break;
			}
		}

		if (start == null)
			throw new Exception("Level does not contain a starting point!");
		Player = new Player(start.Value.x, start.Value.y, Mathf.HALF_PI);
		Player.MoveInput();
	}

	/// <summary>
	/// Load in a level from a Tiled file
	/// </summary>
	/// <param name="tiledFile">Path and name of the Tiled file that belongs with this level</param>
	/// <param name="title">Title that shows up on screen when the level starts</param>
	public Level(string tiledFile, string title)
	{
		this.Title = title;
		LoadTiledFile(tiledFile);
	}

	/// <summary>
	/// Shows/Hides the entire level
	/// </summary>
	/// <param name="visibility"></param>
	public void SetVisibility(bool visibility)
	{
		foreach (Tile tile in _tiles)
		{
			if(tile.GetType() == typeof(Tile)) continue;
			TileWall tw = (TileWall) tile;
			tw.SetVisibility(visibility);
		}

		foreach (Enemy enemy in _enemies)
		{
			enemy.SetVisibility(visibility);
		}
	}

	private void LoadTiledFile(string tiledFile)
	{
		Map levelData = MapParser.ReadMap(TechUtils.LoadAsset(tiledFile));
		if (levelData.Layers == null || levelData.Layers.Length < 1)
			throw new Exception("Tile file " + tiledFile + " does not contain a layer!");
		Layer tileLayer = levelData.Layers[0];

		TilesColumns = tileLayer.Width;
		TilesRows = tileLayer.Height;

		short[,] enemyArray = new short[TilesColumns, TilesRows];
		if (levelData.Layers.Length >= 2)
		{
			Layer enemyLayer = levelData.Layers[1];
			enemyArray = enemyLayer.GetTileArray();
			if (Settings.DebugMode)
			{
				Console.WriteLine("Enemies:");
				for (int row = 0; row < TilesRows; row++)
				{
					for (int col = 0; col < TilesColumns; col++)
					{
						Console.Write(enemyArray[col, row]);
					}

					Console.WriteLine("");
				}
			}
		}

		short[,] tileArray = tileLayer.GetTileArray();
		if (Settings.DebugMode)
		{
			Console.WriteLine("Tiles:");
			for (int row = 0; row < TilesRows; row++)
			{
				for (int col = 0; col < TilesColumns; col++)
				{
					Console.Write(tileArray[col, row]);
				}

				Console.WriteLine("");
			}
		}

		_tiles = new Tile[TilesColumns, TilesRows];
		List<Enemy> tempEnemies = new();
		Vector2? start = null;
		int starts = 0;
		for (int row = 0; row < TilesRows; row++)
		for (int col = 0; col < TilesColumns; col++) //TODO: Replace these switches with auto-loaders that parse the tileset files
		{
			switch (tileArray[col, row])
			{
				case 0:
					_tiles[col, row] = new Tile();
					break;
				case 1:
					_tiles[col, row] = new TileWall(col, row, "square.png");
					break;
				case 2:
					_tiles[col, row] = new TileWall(col, row, "colors.png");
					break;
				case 3:
					_tiles[col, row] = new TileWall(col, row, "checkers.png");
					break;
				case 4:
					_tiles[col, row] = new TileWall(col, row, "diagonal.png");
					break;
				case 5:
					_tiles[col, row] = new TileWall(col, row, "noise.png");
					break;
				case 6:
					_tiles[col, row] = new TileNext(col, row);
					break;
				case 7:
					_tiles[col, row] = new TileStart(col, row);
					start = new Vector2(col + 0.5f, row + 0.5f);
					starts++;
					break;
				case 8:
					_tiles[col, row] = new TileWall(col, row, "plasterBrick.png");
					break;
				case 9:
					_tiles[col, row] = new TileWall(col, row, "plasterWindow.png");
					break;
				default:
					throw new Exception("Level '" + Title + "' Found unexpected items in tileArray: " + tileArray[col, row]);
			}

			switch (enemyArray[col, row])
			{
				case 0:
					break;
				case 10:
					tempEnemies.Add(new Enemy("triangle.png", col + 0.5f, row + 0.5f));
					break;
				case 11:
					tempEnemies.Add(new Enemy("circle.png", col + 0.5f, row + 0.5f));
					break;
				default:
					throw new Exception("Level '" + Title + "' Found unexpected items in enemyArray: " + enemyArray[col, row]);
			}
		}

		_enemies = tempEnemies.ToArray();
		if (start == null || starts != 1)
			throw new Exception("Level '" + Title + "' does not contain exactly one starting point!");
		Player = new Player(start.Value.x, start.Value.y, Mathf.HALF_PI);
		Player.MoveInput();
		SetVisibility(false);
	}

	public void MoveEnemies()
	{
		foreach (Enemy enemy in _enemies)
		{
			enemy.Move();
		}
	}

	public IEnumerable<Enemy> FindVisibleEnemies()
	{
		foreach (Enemy enemy in _enemies)
		{
			enemy.SetVisibility(false);
			if (!(CanPlayerSee(enemy.EnemyPosition) || CanPlayerSee(enemy.Edge1) || CanPlayerSee(enemy.Edge2))) continue;
			enemy.SetVisibility(true);
			yield return enemy;
		}
	}

	private static bool CanPlayerSee(Vector2 target)
	{
		Vector2 toTarget = target - MyGame.CurrentLevel.Player.Position;
		float angleBetween = Angle.Difference(MyGame.CurrentLevel.Player.Heading.Heading(), toTarget.Heading());

		if (angleBetween > MyGame.FieldOfView / 1.5f) //TODO: Find better value for this
			return false; //Skip all enemies that are outside of the FOV + an extra margin

		(TileWall _, Vector2? _, float dist) = TileWall.DDA(MyGame.CurrentLevel.Player.Position, toTarget.Normalize(),
			Mathf.Min(toTarget.Mag(), RENDER_DISTANCE));
		return toTarget.MagSq() < dist*dist;
	}

	public List<TileWall> FindOnscreenTileWalls()
	{
		//Reset all visible tiles
		List<TileWall> onscreenTileWalls = new();
		for (int col = 0; col < TilesColumns; col++)
		for (int row = 0; row < TilesRows; row++)
		{
			if (_tiles[col, row].GetType() == typeof(Tile)) continue;
			TileWall tw = (TileWall) _tiles[col, row];
			tw.SetVisibility(false);
		}

		//Find tiles to render
		//For every x pixel, send out a ray that goes until it has hit a wall or reached the maximum render distance
		for (int px = 0; px < MyGame.StaticWidth; px+=3)
		{
			float rayAngle = (Player.Angle - MyGame.FieldOfView / 2.0f) +
			                 (px / (float) MyGame.StaticWidth) * MyGame.FieldOfView;

			(TileWall tileWall, Vector2? _, float _) = TileWall.DDA(Player.Position, Vector2.FromAngle(Angle.FromRadians(rayAngle)), RENDER_DISTANCE);
			if (tileWall != null && !onscreenTileWalls.Contains(tileWall))
				onscreenTileWalls.Add(tileWall);
		}

		return onscreenTileWalls;
	}

	public Tile GetTileAtPosition(int col, int row)
	{
		return _tiles[col, row];
	}
}
