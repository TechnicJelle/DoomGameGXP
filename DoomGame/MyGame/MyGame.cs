using System;

namespace GXPEngine.MyGame
{
	public class MyGame : Game
	{
		public const int WIDTH = 1280;
		public const int HEIGHT = 720;

		public const float FIELD_OF_VIEW = Mathf.PI / 3.0f;

		private readonly EasyDraw canvas;
		public static Level level;
		private readonly Minimap minimap;

		private const bool USE_TILED = true;

		private MyGame() : base(WIDTH, HEIGHT, false, true, pPixelArt: true)
		{
			//Level
			if (USE_TILED)
			{
				level = new Level("Level01.tmx");
			}
			else
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
				level = new Level(16, 16, map);
			}
#pragma warning restore CS0162

			//Player
			Player player = new Player(8.0f, 4.0f, 0.1f);
			AddChild(player);

			//Render Background
			EasyDraw background = new EasyDraw(WIDTH, HEIGHT, false);
			for (int iy = 0; iy < HEIGHT; iy++)
			{
				if (iy < HEIGHT / 2)
				{
					// Ceiling
					float fac = Map(iy, 0, HEIGHT, 1.0f, 0.4f);
					background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
				}
				else
				{
					//Floor
					background.Stroke((int) Map(iy, 0, HEIGHT, 0, 64));
				}

				background.Line(0, iy, WIDTH, iy);
			}
			AddChild(background);

			//Canvas
			canvas = new EasyDraw(WIDTH, HEIGHT, false);
			canvas.TextAlign(CenterMode.Min, CenterMode.Min);
			AddChild(canvas);

			//Minimap
			minimap = new Minimap(this,0, 0, 200, 200, level);

			//Debug overlay

			Console.WriteLine("MyGame initialized");
		}

		// For every game object, Update is called every frame, by the engine:
		private void Update()
		{
			canvas.ClearTransparent();

			Player.MoveInput();
			minimap.Update();

			level.Render(canvas, minimap);

			canvas.Stroke(255);
			canvas.Text(currentFps.ToString(), 200, 10);
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