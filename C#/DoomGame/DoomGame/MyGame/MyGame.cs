using System;
using System.Collections.Generic;

namespace GXPEngine.MyGame
{
	public class MyGame : Game
	{
		private readonly string _map = "";

		private float _fPlayerX = 8.0f;
		private float _fPlayerY = 8.0f;
		private float _fPlayerA = 0.1f;

		private const int N_MAP_WIDTH = 16;
		private const int N_MAP_HEIGHT = 16;

		private const float F_FOV = Mathf.PI / 4f;
		private const float F_DEPTH = 16.0f;

		private const float F_ROTATION_SPEED = 0.005f;
		private const float F_MOVE_SPEED = 0.005f;

		private readonly EasyDraw _canvas;

		private MyGame() : base(1280, 720, false, true, pPixelArt: true)
		{
			_map += "################";
			_map += "#.#............#";
			_map += "#.#......#######";
			_map += "#.###..........#";
			_map += "#..............#";
			_map += "#..............#";
			_map += "#...#..........#";
			_map += "#...#..........#";
			_map += "#..............#";
			_map += "#........#.....#";
			_map += "#........#.....#";
			_map += "#..............#";
			_map += "#..............#";
			_map += "#...###........#";
			_map += "#..............#";
			_map += "################";

			// Draw some things on a canvas:
			_canvas = new EasyDraw(width, height);
			_canvas.TextAlign(CenterMode.Min, CenterMode.Min);

			//Render Background
			EasyDraw background = new EasyDraw(width, height);
			for (int iy = 0; iy < height; iy++)
			{
				if (iy < height / 2)
				{
					// Ceiling
					float fac = Map(iy, 0, height, 1.0f, 0.4f);
					background.Stroke(0, (int) (150 * fac), (int) (255 * fac));
				}
				else
				{
					//Floor
					background.Stroke((int) Map(iy, 0, height, 0, 64));
				}

				background.Line(0, iy, width, iy);
			}

			// Add the canvas to the engine to display it:
			AddChild(background);
			AddChild(_canvas);
			Console.WriteLine("MyGame initialized");
		}

		// For every game object, Update is called every frame, by the engine:
		private void Update()
		{
			_canvas.ClearTransparent();

			if (Input.GetKey(Key.A))
			{
				_fPlayerA -= F_ROTATION_SPEED * Time.deltaTime;
			}

			if (Input.GetKey(Key.D))
			{
				_fPlayerA += F_ROTATION_SPEED * Time.deltaTime;
			}

			if (Input.GetKey(Key.W))
			{
				_fPlayerX += Mathf.Sin(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
				_fPlayerY += Mathf.Cos(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
				if (_map[(int) _fPlayerY * N_MAP_WIDTH + (int) _fPlayerX] == '#')
				{
					_fPlayerX -= Mathf.Sin(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
					_fPlayerY -= Mathf.Cos(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}

			if (Input.GetKey(Key.S))
			{
				_fPlayerX -= Mathf.Sin(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
				_fPlayerY -= Mathf.Cos(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
				if (_map[(int) _fPlayerY * N_MAP_WIDTH + (int) _fPlayerX] == '#')
				{
					_fPlayerX += Mathf.Sin(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
					_fPlayerY += Mathf.Cos(_fPlayerA) * F_MOVE_SPEED * Time.deltaTime;
				}
			}

			//Render Walls
			for (int ix = 0; ix < width; ix++)
			{
				float fRayAngle = (_fPlayerA - F_FOV / 2.0f) + (ix / (float) width) * F_FOV;

				float fDistanceToWall = 0.0f;
				bool bHitWall = false;
				bool bBoundary = false;

				float fEyeX = Mathf.Sin(fRayAngle);
				float fEyeY = Mathf.Cos(fRayAngle);

				while (!bHitWall && fDistanceToWall < F_DEPTH)
				{
					fDistanceToWall += 0.01f;

					int nTestX = (int) (_fPlayerX + fEyeX * fDistanceToWall);
					int nTestY = (int) (_fPlayerY + fEyeY * fDistanceToWall);

					if (nTestX < 0 || nTestX >= N_MAP_WIDTH | nTestY < 0 || nTestY >= N_MAP_HEIGHT)
					{
						//Ray has gone out of map bounds
						bHitWall = true;
						fDistanceToWall = F_DEPTH;
					}
					else
					{
						if (_map[nTestY * N_MAP_WIDTH + nTestX] != '#') continue;
						bHitWall = true;

						List<(float, float)> p = new List<(float, float)>(); // distance, dot product
						for (int tx = 0; tx < 2; tx++)
						{
							for (int ty = 0; ty < 2; ty++)
							{
								float vy = (float) nTestY + ty - _fPlayerY;
								float vx = (float) nTestX + tx - _fPlayerX;
								float d = Mathf.Sqrt(vx * vx + vy * vy);
								float dot = (fEyeX * vx / d) + (fEyeY * vy / d);
								p.Add((d, dot));
							}
						}

						p.Sort((left, right) => left.Item1.CompareTo(right.Item1));

						const float fBound = 0.001f;
						if (Mathf.Acos(p[0].Item2) < fBound) bBoundary = true;
						if (Mathf.Acos(p[1].Item2) < fBound) bBoundary = true;
						p = null;
					}
				}

				float fCeiling = height / 2.0f - height / fDistanceToWall;
				float fFloor = height - fCeiling;

				if (bBoundary)
				{
					_canvas.Stroke(0);
				}
				else
				{
					_canvas.Stroke((int) Map(fDistanceToWall, 0, F_DEPTH, 255, 0));
				}

				_canvas.Line(ix, fCeiling, ix, fFloor);
			}

			_canvas.Text(currentFps.ToString(), 10, 10);

			_canvas.Stroke(255, 0, 0);
		}

		private static void Main() // Main() is the first method that's called when the program is run
		{
			new MyGame().Start(); // Create a "MyGame" and start it
		}

		private static float Map(float value,
			float start1, float stop1,
			float start2, float stop2)
		{
			return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
		}
	}
}