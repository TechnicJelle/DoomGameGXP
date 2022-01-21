namespace GXPEngine.MyGame
{
	public class Minimap
	{
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly float originX;
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly float originY;
		private readonly EasyDraw layerPlayer;
		private readonly EasyDraw layerLevel;
		private readonly EasyDraw layerDebug;

		private readonly float w, h;
		private readonly Level level;

		public Minimap(MyGame game, int x, int y, int width, int height, Level level)
		{
			originX = x;
			originY = y;
			this.level = level;

			layerLevel = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			game.AddChild(layerLevel);

			layerPlayer = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			game.AddChild(layerPlayer);

			layerDebug = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			game.AddChild(layerDebug);

			w = (layerLevel.width-1.0f) / level.tilesColumns;
			h = (layerLevel.height-1.0f) / level.tilesRows;
		}

		public void Update()
		{
			//layer level
			layerLevel.ClearTransparent();
			layerLevel.Stroke(0);
			layerLevel.StrokeWeight(1);
			layerLevel.ShapeAlign(CenterMode.Min, CenterMode.Min);
			for (int col = 0; col < level.tilesColumns; col++)
			for (int row = 0; row < level.tilesRows; row++)
			{
				Tile t = level.GetTileAtPosition(col, row);
				if (t.GetType() != typeof(TileWall)) continue;
				TileWall tw = (TileWall) t;
				layerLevel.Fill(tw.visible ? 255 : 0);
				layerLevel.Rect(col * w, row * h, w, h);

				tw.tempSprite.x = col * w + w/2.0f;
				tw.tempSprite.y = row * h + h/2.0f;
				tw.tempSprite.scaleX = w / tw.tempSprite.width;
				tw.tempSprite.scaleY = h / tw.tempSprite.height;
				layerLevel.DrawSprite(tw.tempSprite);
				tw.tempSprite.scaleX = 1.0f;
				tw.tempSprite.scaleY = 1.0f;
			}

			//layer player
			layerPlayer.ClearTransparent();
			layerPlayer.NoStroke();
			layerPlayer.Fill(255, 0, 0);
			//draw point on position and cone in player direction
			layerPlayer.Ellipse(
				Player.position.x * w,
				Player.position.y * h,
				8, 8);
			//TODO: Make player on minimap rotate the correct way

			//layer debug
			layerDebug.ClearTransparent();
		}

		public void DebugNoFill()
		{
			layerDebug.NoFill();
		}

		public void DebugFill(int grayScale, int alpha=255)
		{
			layerDebug.Fill(grayScale, alpha);
		}

		public void DebugFill(int red, int green, int blue, int alpha=255)
		{
			layerDebug.Fill(red, green, blue, alpha);
		}

		public void DebugNoStroke()
		{
			layerDebug.NoStroke();
		}

		public void DebugStroke(int grayScale, int alpha=255)
		{
			layerDebug.Stroke(grayScale, alpha);
		}

		public void DebugStroke(int red, int green, int blue, int alpha=255)
		{
			layerDebug.Stroke(red, green, blue, alpha);
		}

		public void DebugStrokeWeight(float width)
		{
			layerDebug.StrokeWeight(width);
		}


		public void DebugLine(float x1, float y1, float x2, float y2)
		{
			layerDebug.Line(x1 * w,  y1 * h, x2 * w,  y2 * h);
		}

		public void DebugCircle(float x, float y, float diameter)
		{
			layerDebug.Ellipse(x * w, y * h, diameter, diameter);
		}
	}
}