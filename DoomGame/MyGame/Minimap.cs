namespace GXPEngine.MyGame
{
	public class Minimap
	{
		// ReSharper disable twice PrivateFieldCanBeConvertedToLocalVariable
		private readonly float _originX, _originY;
		private readonly EasyDraw _layerPlayer;

		private readonly float _w, _h;
		
		public Minimap(MyGame game, int x, int y, int width, int height, Level level)
		{
			_originX = x;
			_originY = y;
			
			EasyDraw layerLevel = new EasyDraw(width, height, false)
			{
				x = _originX,
				y = _originY,
			};
			
			layerLevel.Fill(255);
			layerLevel.Stroke(0);
			layerLevel.StrokeWeight(1);
			layerLevel.ShapeAlign(CenterMode.Min, CenterMode.Min);
			
			_w = (layerLevel.width-1.0f) / level.TilesWidth;
			_h = (layerLevel.height-1.0f) / level.TilesHeight;
			for (int iy = 0; iy < level.TilesHeight; iy++)
			for (int ix = 0; ix < level.TilesWidth; ix++)
			{
				if (level.GetTileAtPosition(ix, iy).Type == MyGame.TileType.Wall)
					layerLevel.Rect(ix * _w, iy * _h, _w, _h);
			}

			game.AddChild(layerLevel);
			
			_layerPlayer = new EasyDraw(width, height, false)
			{
				x = _originX,
				y = _originY,
			};
			game.AddChild(_layerPlayer);
		}

		public void Update()
		{
			_layerPlayer.ClearTransparent();
			_layerPlayer.NoStroke();
			_layerPlayer.Fill(255, 0, 0);
			//draw point on position and cone in player direction
			_layerPlayer.Ellipse(
				Player.PlayerX * _w, 
				Player.PlayerY * _h, 
				8, 8);
		}
	}
}