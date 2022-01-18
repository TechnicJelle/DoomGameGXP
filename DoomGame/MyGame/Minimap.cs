namespace GXPEngine.MyGame
{
	public class Minimap
	{
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly float _originX;
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly float _originY;
		private readonly EasyDraw _layerPlayer;
		private readonly EasyDraw _layerLevel;
		private readonly EasyDraw _layerDebug;

		private readonly float _w, _h;
		private readonly Level _level;

		public Minimap(MyGame game, int x, int y, int width, int height, Level level)
		{
			_originX = x;
			_originY = y;
			_level = level;

			_layerLevel = new EasyDraw(width, height, false)
			{
				x = _originX,
				y = _originY,
			};
			game.AddChild(_layerLevel);

			_layerPlayer = new EasyDraw(width, height, false)
			{
				x = _originX,
				y = _originY,
			};
			game.AddChild(_layerPlayer);

			_layerDebug = new EasyDraw(width, height, false)
			{
				x = _originX,
				y = _originY,
			};
			game.AddChild(_layerDebug);

			_w = (_layerLevel.width-1.0f) / level.TilesWidth;
			_h = (_layerLevel.height-1.0f) / level.TilesHeight;
		}

		public void Update()
		{
			//layer level
			_layerLevel.ClearTransparent();
			_layerLevel.Stroke(0);
			_layerLevel.StrokeWeight(1);
			_layerLevel.ShapeAlign(CenterMode.Min, CenterMode.Min);
			for (int iy = 0; iy < _level.TilesHeight; iy++)
			for (int ix = 0; ix < _level.TilesWidth; ix++)
			{
				Tile t = _level.GetTileAtPosition(ix, iy);
				if (t.Type != MyGame.TileType.Wall) continue;
				_layerLevel.Fill(t.Visible ? 255 : 0);

				_layerLevel.Rect(ix * _w, iy * _h, _w, _h);
				//TODO: By switching these ix and iy around, the minimap is drawn in such a way that A/D rotate the view the correct way, unlike currently
			}

			//layer player
			_layerPlayer.ClearTransparent();
			_layerPlayer.NoStroke();
			_layerPlayer.Fill(255, 0, 0);
			//draw point on position and cone in player direction
			_layerPlayer.Ellipse(
				Player.PlayerX * _w,
				Player.PlayerY * _h,
				8, 8);

			//layer debug
			_layerDebug.ClearTransparent();
		}

		public void DebugNoFill()
		{
			_layerDebug.NoFill();
		}

		public void DebugFill(int grayScale, int alpha=255)
		{
			_layerDebug.Fill(grayScale, alpha);
		}

		public void DebugFill(int red, int green, int blue, int alpha=255)
		{
			_layerDebug.Fill(red, green, blue, alpha);
		}

		public void DebugNoStroke()
		{
			_layerDebug.NoStroke();
		}

		public void DebugStroke(int grayScale, int alpha=255)
		{
			_layerDebug.Stroke(grayScale, alpha);
		}

		public void DebugStroke(int red, int green, int blue, int alpha=255)
		{
			_layerDebug.Stroke(red, green, blue, alpha);
		}

		public void DebugStrokeWeight(float width)
		{
			_layerDebug.StrokeWeight(width);
		}


		public void DebugLine(float x1, float y1, float x2, float y2)
		{
			_layerDebug.Line(x1 * _w,  y1 * _h, x2 * _w,  y2 * _h);
		}

		public void DebugCircle(float x, float y, float diameter)
		{
			_layerDebug.Ellipse(x * _w, y * _h, diameter, diameter);
		}
	}
}