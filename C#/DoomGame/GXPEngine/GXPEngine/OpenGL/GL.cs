using System;
using System.Security;
using System.Runtime.InteropServices;

namespace GXPEngine.OpenGL {
	
	public class GL
	{
		
		//----------------------------------------------------------------------------------------------------------------------
		//														openGL
		//----------------------------------------------------------------------------------------------------------------------
		public const int TEXTURE_2D					= 0x0DE1;
		public const int BLEND						= 0x0BE2;
		public const int MODELVIEW					= 0x1700;		
		public const int PROJECTION					= 0x1701;
		public const int COLOR_BUFFER_BIT			= 0x4000;
		public const int QUADS						= 0x0007;
		public const int TRIANGLES					= 0x0004;
		public const int LINES						= 0x0001;
		public const int TEXTURE_MIN_FILTER			= 0x2801;
		public const int TEXTURE_MAG_FILTER			= 0x2800;
		public const int LINEAR						= 0x2601;
		public const int TEXTURE_WRAP_S				= 0x2802;
		public const int TEXTURE_WRAP_T				= 0x2803;
		public const int CLAMP						= 0x2900;
		public const int GL_CLAMP_TO_EDGE_EXT 		= 0x812F;
		public const int RGBA						= 0x1908;
		public const int BGRA						= 0x80E1;
		public const int UNSIGNED_BYTE				= 0x1401;
		public const int PERSPECTIVE_CORRECTION	 	= 0x0C50;
		public const int FASTEST					= 0x1101;
		public const int NICEST						= 0x1102;
		public const int NEAREST					= 0x2600;
		public const int POLYGON_SMOOTH				= 0x0B41;
		public const int MULTISAMPLE				= 0x809D;
		public const int FLOAT						= 0x1406;
		public const int UNSIGNED_INT				= 0x1405;
		public const int VERTEX_ARRAY				= 0x8074;
		public const int INT						= 0x1404;
		public const int DOUBLE						= 0x140A;
		public const int INDEX_ARRAY				= 0x8077;
		public const int TEXTURE_COORD_ARRAY		= 0x8078;
		public const int SCISSOR_TEST				= 0x0C11;
		public const int MAX_TEXTURE_SIZE 			= 0x0D33;
		public const int ZERO						= 0x0000;
		public const int ONE						= 0x0001;
		public const int SRC_COLOR					= 0x0300;
		public const int ONE_MINUS_SRC_COLOR		= 0x0301;
		public const int DST_COLOR					= 0x0306;
		public const int ONE_MINUS_DST_COLOR		= 0x0307;
		public const int SRC_ALPHA					= 0x0302;
		public const int ONE_MINUS_SRC_ALPHA		= 0x0303;
		public const int DST_ALPHA					= 0x0304;
		public const int ONE_MINUS_DST_ALPHA		= 0x0305;
		public const int CONSTANT_COLOR				= 0x8001;
		public const int ONE_MINUS_CONSTANT_COLOR 	= 0x8002;
		public const int CONSTANT_ALPHA				= 0x8003;
		public const int ONE_MINUS_CONSTANT_ALPHA 	= 0x8004;
		public const int SRC_ALPHA_SATURATE			= 0x0308;
		public const int MIN						= 0x8007;
		public const int MAX						= 0x8008;
		public const int FUNC_ADD					= 0x8006;
		public const int FUNC_SUBTRACT				= 0x800A;
		public const int FUNC_REVERSE_SUBTRACT		= 0x800B;
		public const int GL_REPEAT 					= 0x2901;

		[DllImport("opengl32.dll", EntryPoint="glEnable")]
		public static extern void Enable(int cap);
		[DllImport("opengl32.dll", EntryPoint="glDisable")]
		public static extern void Disable(int cap);
		[DllImport("opengl32.dll", EntryPoint="glBlendFunc")]
		public static extern void BlendFunc(int sourceFactor, int destFactor);
		[DllImport("opengl32.dll", EntryPoint="glBlendEquation")]
		public static extern void BlendEquation(int mode);
		[DllImport("opengl32.dll", EntryPoint="glClearColor")]
		public static extern void ClearColor(float r, float g, float b, float a);
		[DllImport("opengl32.dll", EntryPoint="glMatrixMode")]
		public static extern void MatrixMode(int mode);
		[DllImport("opengl32.dll", EntryPoint="glLoadIdentity")]
		public static extern void LoadIdentity();
		[DllImport("opengl32.dll", EntryPoint="glOrtho")]
		public static extern void Ortho(double left, double right, double top, double bottom, double near, double far);
		[DllImport("opengl32.dll", EntryPoint="glClear")]
		public static extern void Clear(int mask);
		[DllImport("opengl32.dll", EntryPoint="glColor4ub")]
		public static extern void Color4ub(byte r, byte g, byte b, byte a);
		[DllImport("opengl32.dll", EntryPoint="glPushMatrix")]
		public static extern void PushMatrix();
		[DllImport("opengl32.dll", EntryPoint="glMultMatrixf")]
		public static extern void MultMatrixf(float[] matrix);
		[DllImport("opengl32.dll", EntryPoint="glPopMatrix")]
		public static extern void PopMatrix();
		[DllImport("opengl32.dll", EntryPoint="glBegin")]
		public static extern void Begin(int mode);



		[DllImport("opengl32.dll", EntryPoint="glTexCoord2f")]
		public static extern void TexCoord2f(float u, float v);
		[DllImport("opengl32.dll", EntryPoint="glVertex2f")]
		public static extern void Vertex2f(float x, float y);
		[DllImport("opengl32.dll", EntryPoint="glVertex3f")]
		public static extern void Vertex3f(float x, float y, float z);
		[DllImport("opengl32.dll", EntryPoint="glEnd")]
		public static extern void End();
		[DllImport("opengl32.dll", EntryPoint="glBindTexture")]
		public static extern void BindTexture(int target, int texture);
		[DllImport("opengl32.dll", EntryPoint="glGenTextures")]
		public static extern void GenTextures(int count, int[] textures);
		[DllImport("opengl32.dll", EntryPoint="glTexParameteri")]
		public static extern void TexParameteri(int target, int name, int value);
		[DllImport("opengl32.dll", EntryPoint="glTexImage2D")]
		public static extern void TexImage2D(int target, int level, int internalFormat, int width, int height, 
		                                     int border, int format, int type, IntPtr pixels);
		[DllImport("opengl32.dll", EntryPoint="glDeleteTextures")]
		public static extern void DeleteTextures(int count, int[] textures);
		[DllImport("opengl32.dll", EntryPoint="glFinish")]
		public static extern void Flush();
		[DllImport("opengl32.dll", EntryPoint="glFlush")]
		public static extern void Finish();
		[DllImport("opengl32.dll", EntryPoint="glHint")]
		public static extern void Hint(int target, int mode);
		[DllImport("opengl32.dll", EntryPoint="glViewport")]
		public static extern void Viewport(int x, int y, int width, int height);
		[DllImport("opengl32.dll", EntryPoint="glScissor")]
		public static extern void Scissor(int x, int y, int width, int height);
		[DllImport("opengl32.dll", EntryPoint="glVertexPointer")]
		public static extern void VertexPointer(int size, int type, int stride, float[] pointer);
		
		[DllImport("opengl32.dll", EntryPoint="glTexCoordPointer")]
		public static extern void TexCoordPointer(int size, int type, int stride, float[] pointer);
		
		[DllImport("opengl32.dll", EntryPoint="glDrawElements")]
		public static extern void DrawElements(int mode, int count, int type, int[] indices);
		[DllImport("opengl32.dll", EntryPoint="glEnableClientState")]
		public static extern void EnableClientState(int array);
		[DllImport("opengl32.dll", EntryPoint="glArrayElement")]
		public static extern void ArrayElement(int element);
		[DllImport("opengl32.dll", EntryPoint="glDrawArrays")]
		public static extern void DrawArrays(int mode, int offset, int count);
		[DllImport("opengl32.dll", EntryPoint="glDisableClientState")]
		public static extern void DisableClientState(int state);
		[DllImport("opengl32.dll", EntryPoint="glGetError")]
		public static extern int GetError();
		[DllImport("opengl32.dll", EntryPoint="glGetIntegerv")]
		public static extern void GetIntegerv(int name, int[] param);

		[DllImport("opengl32.dll", EntryPoint="glLineWidth")]
		public static extern void LineWidth(float width);

		//----------------------------------------------------------------------------------------------------------------------
		//														GLFW
		//----------------------------------------------------------------------------------------------------------------------
		
		public const int GLFW_OPENED 					= 0x00020001;
		public const int GLFW_WINDOWED             		= 0x00010001;
		public const int GLFW_FULLSCREEN           		= 0x00010002;
		public const int GLFW_ACTIVE               		= 0x00020001;
		public const int GLFW_FSAA_SAMPLES				= 0x0002100E;
		public const int GLFW_MOUSE_CURSOR				= 0x00030001;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		public delegate void GLFWWindowSizeCallback(int width, int height);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		public delegate void GLFWKeyCallback(int key, int action);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		public delegate void GLFWMouseButtonCallback(int button, int action);
		
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSetTime (double time);
		[DllImport("lib/glfw.dll")]
		public static extern double glfwGetTime ();

		[DllImport("lib/glfw.dll")]
		public static extern void glfwPollEvents();
		[DllImport("lib/glfw.dll")]
		public static extern int glfwGetWindowParam(int param);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwInit();
		[DllImport("lib/glfw.dll")]
		public static extern void glfwOpenWindow(int width, int height, int r, int g, int b, int a, int depth, int stencil, int mode);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSetWindowTitle(string title);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSwapInterval(bool mode);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSetWindowSizeCallback(GLFWWindowSizeCallback callback);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwCloseWindow();
		[DllImport("lib/glfw.dll")]
		public static extern void glfwTerminate();				
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSwapBuffers();
		[DllImport("lib/glfw.dll")]
		public static extern bool glfwGetKey(int key);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSetKeyCallback(GLFWKeyCallback callback);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwOpenWindowHint(int name, int value);
		[DllImport("lib/glfw.dll")]
		public static extern bool glfwGetMousePos(out int x, out int y);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwSetMouseButtonCallback(GLFWMouseButtonCallback callback);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwEnable(int property);
		[DllImport("lib/glfw.dll")]
		public static extern void glfwDisable(int property);
	}
}
