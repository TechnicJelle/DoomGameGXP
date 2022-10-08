using GXPEngine.GXPEngine;
using GXPEngine.GXPEngine.AddOns;
using GXPEngine.GXPEngine.Core;

#pragma warning disable CS0162

namespace GXPEngine.MyGame;

public static class Minimap
{
	private static float _originX;
	private static float _originY;
	private static EasyDraw _layerLevel;
	private static EasyDraw _layerPlayer;
	private static EasyDraw _layerEnemies;
	private static EasyDraw _layerDebug;

	private static float _w;
	private static float _h;

	private static bool _firstLaunch = true;

	public static void Setup(int x, int y, int width, int height)
	{
		if (!_firstLaunch)
			ClearAll();
		else
			_firstLaunch = false;

		SetPosition(x, y);

		_layerLevel = new EasyDraw(width, height, false)
		{
			x = _originX,
			y = _originY,
		};
		Game.main.AddChild(_layerLevel);

		_layerPlayer = new EasyDraw(width, height, false)
		{
			x = _originX,
			y = _originY,
		};
		Game.main.AddChild(_layerPlayer);

		_layerEnemies = new EasyDraw(width, height, false)
		{
			x = _originX,
			y = _originY,
		};
		Game.main.AddChild(_layerEnemies);

		if (Settings.DebugMode)
		{
			_layerDebug = new EasyDraw(width, height, false)
			{
				x = _originX,
				y = _originY,
			};
			Game.main.AddChild(_layerDebug);
		}
	}

	public static void DrawCurrentLevel() {
		_w = (_layerLevel.width-1.0f) / MyGame.CurrentLevel.TilesColumns;
		_h = (_layerLevel.height-1.0f) / MyGame.CurrentLevel.TilesRows;

		UpdateLevel();
		UpdatePlayer();
	}

	public static void UpdateLevel()
	{
		// layerLevel.ClearTransparent();
		_layerLevel.Clear(63);
		for (int col = 0; col < MyGame.CurrentLevel.TilesColumns; col++)
		for (int row = 0; row < MyGame.CurrentLevel.TilesRows; row++)
		{
			Tile t = MyGame.CurrentLevel.GetTileAtPosition(col, row);
			if (t.GetType() == typeof(Tile)) continue;
			TileWall tw = (TileWall) t;
			DrawSprite(_layerLevel, tw.MinimapTexture, col + 0.5f, row + 0.5f);
		}
	}

	private static void DrawSprite(Canvas layer, Sprite sprite, float x, float y)
	{
		sprite.x = x * _w;
		sprite.y = y * _h;
		float tempSpriteScaleX = sprite.scaleX;
		float tempSpriteScaleY = sprite.scaleY;
		sprite.scaleX = _w / sprite.width;
		sprite.scaleY = _h / sprite.height;
		layer.DrawSprite(sprite);
		sprite.scaleX = tempSpriteScaleX;
		sprite.scaleY = tempSpriteScaleY;
	}

	public static void UpdatePlayer()
	{
		_layerPlayer.ClearTransparent();

		_layerPlayer.NoStroke();
		_layerPlayer.Fill(255, 0, 0);
		_layerPlayer.Ellipse(
			MyGame.CurrentLevel.Player.Position.x * _w,
			MyGame.CurrentLevel.Player.Position.y * _h,
			8, 8);

		//Player Heading
		Vector2 playerLooking = MyGame.CurrentLevel.Player.Heading + MyGame.CurrentLevel.Player.Position;
		_layerPlayer.Stroke(255, 0, 0);
		_layerPlayer.Line(MyGame.CurrentLevel.Player.Position.x * _w, MyGame.CurrentLevel.Player.Position.y * _h, playerLooking.x * _w, playerLooking.y * _h);

		//TODO: Player FOV cone
	}

	public static void ClearEnemies()
	{
		_layerEnemies.ClearTransparent();
	}

	public static void DrawEnemy(Sprite sprite, Vector2 position)
	{
		DrawSprite(_layerEnemies, sprite, position.x, position.y);
	}

	public static void ClearDebug()
	{
		if (Settings.DebugMode)
			_layerDebug.ClearTransparent();
	}

	public static void ReOverlay()
	{
		Game.main.RemoveChild(_layerLevel);
		Game.main.RemoveChild(_layerPlayer);
		Game.main.RemoveChild(_layerEnemies);

		Game.main.AddChild(_layerLevel);
		Game.main.AddChild(_layerPlayer);
		Game.main.AddChild(_layerEnemies);

		if (!Settings.DebugMode) return;
		Game.main.RemoveChild(_layerDebug);
		Game.main.AddChild(_layerDebug);
	}

	private static void SetPosition(float x, float y)
	{
		_originX = x;
		_originY = y;
	}

	public static void DebugNoFill()
	{
		_layerDebug.NoFill();
	}

	public static void DebugFill(int grayScale, int alpha=255)
	{
		_layerDebug.Fill(grayScale, alpha);
	}

	public static void DebugFill(int red, int green, int blue, int alpha=255)
	{
		_layerDebug.Fill(red, green, blue, alpha);
	}

	public static void DebugNoStroke()
	{
		_layerDebug.NoStroke();
	}

	public static void DebugStroke(int grayScale, int alpha=255)
	{
		_layerDebug.Stroke(grayScale, alpha);
	}

	public static void DebugStroke(int red, int green, int blue, int alpha=255)
	{
		_layerDebug.Stroke(red, green, blue, alpha);
	}

	public static void DebugStrokeWeight(float width)
	{
		_layerDebug.StrokeWeight(width);
	}

	public static void DebugLine(float x1, float y1, float x2, float y2)
	{
		_layerDebug.Line(x1 * _w,  y1 * _h, x2 * _w,  y2 * _h);
	}

	public static void DebugCircle(float x, float y, float diameter)
	{
		_layerDebug.Ellipse(x * _w, y * _h, diameter, diameter);
	}

	public static void ClearAll()
	{
		_layerLevel.ClearTransparent();
		_layerPlayer.ClearTransparent();
		ClearEnemies();
		ClearDebug();

		Game.main.RemoveChild(_layerLevel);
		Game.main.RemoveChild(_layerPlayer);
		Game.main.RemoveChild(_layerEnemies);
		if(Settings.DebugMode)
			Game.main.RemoveChild(_layerDebug);
	}
}
