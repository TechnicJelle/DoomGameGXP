//#define USE_FMOD_AUDIO
#define STRETCH_ON_RESIZE

using System;
using GXPEngine.OpenGL;

namespace GXPEngine.Core {

	class WindowSize {
		public static WindowSize instance = new WindowSize();
		public int width, height;
	}
	
	public class GLContext {
		
		const int MAXKEYS = 65535;
		const int MAXBUTTONS = 255;

		private static bool[] keys = new bool[MAXKEYS+1];
		private static bool[] keydown = new bool[MAXKEYS+1];
		private static bool[] keyup = new bool[MAXKEYS+1];
		private static bool[] buttons = new bool[MAXBUTTONS+1];
		private static bool[] mousehits = new bool[MAXBUTTONS+1];
		private static bool[] mouseup = new bool[MAXBUTTONS+1]; //mouseup kindly donated by LeonB
		private static int keyPressedCount = 0;
		private static bool anyKeyDown = false;

		public static int mouseX = 0;
		public static int mouseY = 0;
		
		private Game _owner;
        private static SoundSystem _soundSystem;
		
		private int _targetFrameRate = 60;
		private long _lastFrameTime = 0;
		private long _lastFPSTime = 0;
		private int _frameCount = 0;
		private int _lastFPS = 0;
		private bool _vsyncEnabled = false;

		private static double _realToLogicWidthRatio;
		private static double _realToLogicHeightRatio;

		//------------------------------------------------------------------------------------------------------------------------
		//														RenderWindow()
		//------------------------------------------------------------------------------------------------------------------------
		public GLContext (Game owner) {
			_owner = owner;
			_lastFPS = _targetFrameRate;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Width
		//------------------------------------------------------------------------------------------------------------------------
		public int width {
			get { return WindowSize.instance.width; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Height
		//------------------------------------------------------------------------------------------------------------------------
		public int height {
			get { return WindowSize.instance.height; }
		}

        //------------------------------------------------------------------------------------------------------------------------
        //														SoundSystem
        //------------------------------------------------------------------------------------------------------------------------
        public static SoundSystem soundSystem
        {
            get
            {
				if (_soundSystem == null) {
					InitializeSoundSystem ();
				}
                return _soundSystem;
            }
        }
		
		//------------------------------------------------------------------------------------------------------------------------
		//														setupWindow()
		//------------------------------------------------------------------------------------------------------------------------
		public void CreateWindow(int width, int height, bool fullScreen, bool vSync, int realWidth, int realHeight) {
			// This stores the "logical" width, used by all the game logic:
			WindowSize.instance.width = width;
			WindowSize.instance.height = height;
			_realToLogicWidthRatio = (double)realWidth / width;
			_realToLogicHeightRatio = (double)realHeight / height;
			_vsyncEnabled = vSync;
			
			GL.glfwInit();
			
			GL.glfwOpenWindowHint(GL.GLFW_FSAA_SAMPLES, 8);
			GL.glfwOpenWindow(realWidth, realHeight, 8, 8, 8, 8, 24, 0, (fullScreen?GL.GLFW_FULLSCREEN:GL.GLFW_WINDOWED));
			GL.glfwSetWindowTitle("Game");
			GL.glfwSwapInterval(vSync);
			
			GL.glfwSetKeyCallback(
				(int _key, int _mode) => {
				bool press = (_mode == 1);
				if (press) { keydown[_key] = true; anyKeyDown = true; keyPressedCount++; } 
				else { keyup[_key] = true; keyPressedCount--; }
				keys[_key] = press;
			});
			
			GL.glfwSetMouseButtonCallback(
				(int _button, int _mode) => {
				bool press = (_mode == 1);
				if (press) mousehits[_button] = true;
				else mouseup[_button] = true;
				buttons[_button] = press;
			});

			GL.glfwSetWindowSizeCallback((int newWidth, int newHeight) => {
				GL.Viewport(0, 0, newWidth, newHeight);	
				GL.Enable(GL.MULTISAMPLE);	
				GL.Enable (GL.TEXTURE_2D);
				GL.Enable( GL.BLEND );
				GL.BlendFunc( GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA );
				GL.Hint (GL.PERSPECTIVE_CORRECTION, GL.FASTEST);
				//GL.Enable (GL.POLYGON_SMOOTH);
				GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

				// Load the basic projection settings:
				GL.MatrixMode(GL.PROJECTION);
				GL.LoadIdentity();

#if STRETCH_ON_RESIZE
				_realToLogicWidthRatio = (double)newWidth / WindowSize.instance.width;
				_realToLogicHeightRatio = (double)newHeight / WindowSize.instance.height;
#endif
				// Here's where the conversion from logical width/height to real width/height happens: 
				GL.Ortho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);
#if !STRETCH_ON_RESIZE
				lock (WindowSize.instance) {
					WindowSize.instance.width = (int)(newWidth/_realToLogicWidthRatio);
					WindowSize.instance.height = (int)(newHeight/_realToLogicHeightRatio);
				}
#endif

				if (Game.main!=null) {
					Game.main.RenderRange=new Rectangle(0,0,WindowSize.instance.width,WindowSize.instance.height);
				}
			});
			InitializeSoundSystem ();
		}

		private static void InitializeSoundSystem() {
#if USE_FMOD_AUDIO
			_soundSystem = new FMODSoundSystem();
#else
			_soundSystem = new SoloudSoundSystem();
#endif
			_soundSystem.Init();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														ShowCursor()
		//------------------------------------------------------------------------------------------------------------------------
		public void ShowCursor (bool enable)
		{
			if (enable) {
				GL.glfwEnable(GL.GLFW_MOUSE_CURSOR);
			} else {
				GL.glfwDisable(GL.GLFW_MOUSE_CURSOR);
			}
		}

		public void SetVSync(bool enableVSync) {
			_vsyncEnabled = enableVSync;
			GL.glfwSwapInterval(_vsyncEnabled);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetScissor()
		//------------------------------------------------------------------------------------------------------------------------
		public void SetScissor(int x, int y, int width, int height) {
			if ((width == WindowSize.instance.width) && (height == WindowSize.instance.height)) {
				GL.Disable(GL.SCISSOR_TEST);
			} else {
				GL.Enable(GL.SCISSOR_TEST);
			}

			GL.Scissor(
				(int)(x*_realToLogicWidthRatio), 
				(int)(y*_realToLogicHeightRatio), 
				(int)(width*_realToLogicWidthRatio), 
				(int)(height*_realToLogicHeightRatio)
			);
			//GL.Scissor(x, y, width, height);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Close()
		//------------------------------------------------------------------------------------------------------------------------
		public void Close() {
            _soundSystem.Deinit();
            GL.glfwCloseWindow();
			GL.glfwTerminate();
			System.Environment.Exit(0);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Run()
		//------------------------------------------------------------------------------------------------------------------------
		public void Run() {
            GL.glfwSetTime(0.0);
			do {
				if (_vsyncEnabled || (Time.time - _lastFrameTime > (1000 / _targetFrameRate))) {
					_lastFrameTime = Time.time;
					
					//actual fps count tracker
					_frameCount++;
					if (Time.time - _lastFPSTime > 1000) {
						_lastFPS = (int)(_frameCount / ((Time.time -_lastFPSTime) / 1000.0f));
						_lastFPSTime = Time.time;
						_frameCount = 0;
					}
					
					UpdateMouseInput();
					_owner.Step();
                    _soundSystem.Step();
					
					ResetHitCounters();
					Display();
					
					Time.newFrame ();
					GL.glfwPollEvents();
				}
				
				
			} while (GL.glfwGetWindowParam(GL.GLFW_ACTIVE) == 1);
		}
		
		
		//------------------------------------------------------------------------------------------------------------------------
		//														display()
		//------------------------------------------------------------------------------------------------------------------------
		private void Display () {
			GL.Clear(GL.COLOR_BUFFER_BIT);
			
			GL.MatrixMode(GL.MODELVIEW);
			GL.LoadIdentity();
			
			_owner.Render(this);

			GL.glfwSwapBuffers();
			if (GetKey(Key.ESCAPE)) this.Close();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetColor()
		//------------------------------------------------------------------------------------------------------------------------
		public void SetColor (byte r, byte g, byte b, byte a) {
			GL.Color4ub(r, g, b, a);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														PushMatrix()
		//------------------------------------------------------------------------------------------------------------------------
		public void PushMatrix(float[] matrix) {
			GL.PushMatrix ();
			GL.MultMatrixf (matrix);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														PopMatrix()
		//------------------------------------------------------------------------------------------------------------------------
		public void PopMatrix() {
			GL.PopMatrix ();
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														DrawQuad()
		//------------------------------------------------------------------------------------------------------------------------
		public void DrawQuad(float[] vertices, float[] uv) {
			GL.EnableClientState( GL.TEXTURE_COORD_ARRAY );
			GL.EnableClientState( GL.VERTEX_ARRAY );
			GL.TexCoordPointer( 2, GL.FLOAT, 0, uv);
			GL.VertexPointer( 2, GL.FLOAT, 0, vertices);
			GL.DrawArrays(GL.QUADS, 0, 4);
			GL.DisableClientState(GL.VERTEX_ARRAY);
			GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);			
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetKey()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetKey(int key) {
			return keys[key];
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetKeyDown()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetKeyDown(int key) {
			return keydown[key];
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetKeyUp()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetKeyUp(int key) {
			return keyup[key];
		}
		
		public static bool AnyKey() {
			return keyPressedCount > 0;
		}

		public static bool AnyKeyDown() {
			return anyKeyDown;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetMouseButton()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetMouseButton(int button) {
			return buttons[button];
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														GetMouseButtonDown()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetMouseButtonDown(int button) {
			return mousehits[button];
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetMouseButtonUp()
		//------------------------------------------------------------------------------------------------------------------------
		public static bool GetMouseButtonUp(int button) {
			return mouseup[button];
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														ResetHitCounters()
		//------------------------------------------------------------------------------------------------------------------------
		public static void ResetHitCounters() {
			Array.Clear (keydown, 0, MAXKEYS);
			Array.Clear (keyup, 0, MAXKEYS);
			Array.Clear (mousehits, 0, MAXBUTTONS);
			Array.Clear (mouseup, 0, MAXBUTTONS);
			anyKeyDown = false;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														UpdateMouseInput()
		//------------------------------------------------------------------------------------------------------------------------
		public static void UpdateMouseInput() {
			GL.glfwGetMousePos(out mouseX, out mouseY);
			mouseX = (int)(mouseX / _realToLogicWidthRatio);
			mouseY = (int)(mouseY / _realToLogicHeightRatio);
		}
		
		public int currentFps {
			get { return _lastFPS; }
		}
		
		public int targetFps {
			get { return _targetFrameRate; }
			set {
				if (value < 1) {
					_targetFrameRate = 1;
				} else {
					_targetFrameRate = value;
				}
			}
		}
		
	}	
	
}