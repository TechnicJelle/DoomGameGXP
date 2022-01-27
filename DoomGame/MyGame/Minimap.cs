using GXPEngine.Core;
#pragma warning disable CS0162

namespace GXPEngine.MyGame
{
	public static class Minimap
	{
		private static float originX;
		private static float originY;
		private static EasyDraw layerLevel;
		private static EasyDraw layerPlayer;
		private static EasyDraw layerEnemies;
		private static EasyDraw layerDebug;

		private static float w;
		private static float h;

		private static bool firstLaunch = true;

		public static void Setup(int x, int y, int width, int height)
		{
			if (!firstLaunch)
				ClearAll();
			else
				firstLaunch = false;

			SetPosition(x, y);

			layerLevel = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			Game.main.AddChild(layerLevel);

			layerPlayer = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			Game.main.AddChild(layerPlayer);

			layerEnemies = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			Game.main.AddChild(layerEnemies);

			if (MyGame.DEBUG_MODE)
			{
				layerDebug = new EasyDraw(width, height, false)
				{
					x = originX,
					y = originY,
				};
				Game.main.AddChild(layerDebug);
			}
		}

		public static void DrawCurrentLevel() {
			w = (layerLevel.width-1.0f) / MyGame.currentLevel.tilesColumns;
			h = (layerLevel.height-1.0f) / MyGame.currentLevel.tilesRows;

			UpdateLevel();
			UpdatePlayer();
		}

		public static void UpdateLevel()
		{
			// layerLevel.ClearTransparent();
			layerLevel.Clear(63);
			for (int col = 0; col < MyGame.currentLevel.tilesColumns; col++)
			for (int row = 0; row < MyGame.currentLevel.tilesRows; row++)
			{
				Tile t = MyGame.currentLevel.GetTileAtPosition(col, row);
				if (t.GetType() == typeof(Tile)) continue;
				TileWall tw = (TileWall) t;
				DrawSprite(layerLevel, tw.minimapTexture, col + 0.5f, row + 0.5f);
			}
		}

		private static void DrawSprite(Canvas layer, Sprite sprite, float x, float y)
		{
			sprite.x = x * w;
			sprite.y = y * h;
			float tempSpriteScaleX = sprite.scaleX;
			float tempSpriteScaleY = sprite.scaleY;
			sprite.scaleX = w / sprite.width;
			sprite.scaleY = h / sprite.height;
			layer.DrawSprite(sprite);
			sprite.scaleX = tempSpriteScaleX;
			sprite.scaleY = tempSpriteScaleY;
		}

		public static void UpdatePlayer()
		{
			layerPlayer.ClearTransparent();

			layerPlayer.NoStroke();
			layerPlayer.Fill(255, 0, 0);
			layerPlayer.Ellipse(
				MyGame.currentLevel.player.position.x * w,
				MyGame.currentLevel.player.position.y * h,
				8, 8);

			//Player Heading
			Vector2 playerLooking = MyGame.currentLevel.player.heading.Copy().Add(MyGame.currentLevel.player.position);
			layerPlayer.Stroke(255, 0, 0);
			layerPlayer.Line(MyGame.currentLevel.player.position.x * w, MyGame.currentLevel.player.position.y * h, playerLooking.x * w, playerLooking.y * h);

			//TODO: Player FOV cone
		}

		public static void ClearEnemies()
		{
			layerEnemies.ClearTransparent();
		}

		public static void DrawEnemy(Sprite sprite, Vector2 position)
		{
			DrawSprite(layerEnemies, sprite, position.x, position.y);
		}

		public static void ClearDebug()
		{
			if (MyGame.DEBUG_MODE)
				layerDebug.ClearTransparent();
		}

		public static void ReOverlay()
		{
			Game.main.RemoveChild(layerLevel);
			Game.main.RemoveChild(layerPlayer);
			Game.main.RemoveChild(layerEnemies);

			Game.main.AddChild(layerLevel);
			Game.main.AddChild(layerPlayer);
			Game.main.AddChild(layerEnemies);

			if (!MyGame.DEBUG_MODE) return;
			Game.main.RemoveChild(layerDebug);
			Game.main.AddChild(layerDebug);
		}

		private static void SetPosition(float x, float y)
		{
			originX = x;
			originY = y;
		}

		public static void DebugNoFill()
		{
			layerDebug.NoFill();
		}

		public static void DebugFill(int grayScale, int alpha=255)
		{
			layerDebug.Fill(grayScale, alpha);
		}

		public static void DebugFill(int red, int green, int blue, int alpha=255)
		{
			layerDebug.Fill(red, green, blue, alpha);
		}

		public static void DebugNoStroke()
		{
			layerDebug.NoStroke();
		}

		public static void DebugStroke(int grayScale, int alpha=255)
		{
			layerDebug.Stroke(grayScale, alpha);
		}

		public static void DebugStroke(int red, int green, int blue, int alpha=255)
		{
			layerDebug.Stroke(red, green, blue, alpha);
		}

		public static void DebugStrokeWeight(float width)
		{
			layerDebug.StrokeWeight(width);
		}

		public static void DebugLine(float x1, float y1, float x2, float y2)
		{
			layerDebug.Line(x1 * w,  y1 * h, x2 * w,  y2 * h);
		}

		public static void DebugCircle(float x, float y, float diameter)
		{
			layerDebug.Ellipse(x * w, y * h, diameter, diameter);
		}

		public static void ClearAll()
		{
			layerLevel.ClearTransparent();
			layerPlayer.ClearTransparent();
			ClearEnemies();
			ClearDebug();

			Game.main.RemoveChild(layerLevel);
			Game.main.RemoveChild(layerPlayer);
			Game.main.RemoveChild(layerEnemies);
			if(MyGame.DEBUG_MODE)
				Game.main.RemoveChild(layerDebug);
		}
	}
}