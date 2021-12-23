using System;

namespace GXPEngine.MyGame
{
	public class MyGame : Game
	{
		public const int Width = 1280;
		public const int Height = 720;

		public const float FieldOfView = Mathf.PI / 3.0f;
		public const float ViewDepth = 16.0f;

		public enum TileType
		{
			Empty,
			Wall,
		}

		private readonly Player _player;
		
		public static EasyDraw Canvas;
		public static Level Level;

		private MyGame() : base(Width, Height, false, true, pPixelArt: true)
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

			Level = new Level(16, 16, map);
			_player = new Player(8.0f, 8.0f, 0.1f);

			// Draw some things on a canvas:
			Canvas = new EasyDraw(Width, Height);
			Canvas.TextAlign(CenterMode.Min, CenterMode.Min);

			//Render Background
			EasyDraw background = new EasyDraw(Width, Height);
			for (int iy = 0; iy < Height; iy++)
			{
				if (iy < Height / 2)
				{
					// Ceiling
					float fac = Map(iy, 0, Height, 1.0f, 0.4f);
					background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
				}
				else
				{
					//Floor
					background.Stroke((int) Map(iy, 0, Height, 0, 64));
				}

				background.Line(0, iy, Width, iy);
			}

			// Add the canvas to the engine to display it:
			AddChild(background);
			AddChild(Canvas);
			Console.WriteLine("MyGame initialized");
		}

		// For every game object, Update is called every frame, by the engine:
		private void Update()
		{
			Canvas.ClearTransparent();

			Player.MoveInput();

			Level.Render();
			
			Canvas.Stroke(255);
			Canvas.Text(currentFps.ToString(), 10, 10);
		}

		private static void Main() // Main() is the first method that's called when the program is run
		{
			new MyGame().Start(); // Create a "MyGame" and start it
		}

		public static float Map(float value,
			float start1, float stop1,
			float start2, float stop2)
		{
			return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
		}
	}
}