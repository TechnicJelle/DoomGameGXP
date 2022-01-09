using System;
using System.Collections.Generic;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine {
	/// <summary>
	/// This class can be used to easily draw line based shapes (like arrows and rectangles),
	/// mostly for debug purposes (it is not made for efficiency). 
	/// For each draw call, shapes are drawn for one frame only, after rendering all sprites. 
	/// See the DrawLine method for more information.
	/// </summary>
	public class Gizmos {
		struct DrawLineCall {
			public float x1, y1, x2, y2;
			public byte width;
			public uint color;

			public DrawLineCall(float x1, float y1, float x2, float y2, uint color, byte width) {
				this.x1 = x1;
				this.y1 = y1;
				this.x2 = x2;
				this.y2 = y2;
				this.color = color;
				this.width = width;
			}
		}

		static Gizmos Instance = null;
		static uint defaultColor = 0xffffffff;
		static byte defaultWidth = 3;

		List<DrawLineCall> drawCalls;

		// Private constructor!
		Gizmos() {
			Game.main.OnAfterRender += DrawLines;
			drawCalls = new List<DrawLineCall>();
		}

		/// <summary>
		/// Set a default color and line width for the subsequent draw calls.
		/// The color should be given as a uint consisting of four byte values, in the order ARGB.
		/// </summary>
		public static void SetStyle(uint color, byte width) {
			defaultColor = color;
			defaultWidth = width;
		}

		/// <summary>
		/// Set a default line width for the subsequent draw calls.
		/// </summary>
		/// <param name="width"></param>
		public static void SetWidth(byte width) {
			defaultWidth = width;
		}

		/// <summary>
		/// Set the default color for subsequent draw calls.
		/// The R,G,B color and alpha values should be given as floats between 0 and 1.
		/// </summary>
		public static void SetColor(float R, float G, float B, float alpha = 1) {
			defaultColor = (ToByte(alpha) << 24) + (ToByte(R) << 16) + (ToByte(G) << 8) + (ToByte(B));
		}

		static uint ToByte(float value) {
			return (uint)(Mathf.Clamp(value, 0, 1) * 255);
		}

		/// <summary>
		/// You can call this method from anywhere. A (debug) line will be drawn from (x1,y1) to (x2,y2),
		/// in the space of the given game object. 
		/// If no game object is given, it will be drawn in screen space.
		/// The line will be drawn after drawing all other game objects.
		/// You can give color and line width. If no values are given (=0), the default values are
		/// used. These can be set using SetStyle, SetColor and SetWidth.
		/// </summary>
		public static void DrawLine(float x1, float y1, float x2 = 0, float y2 = 0, GameObject space = null, uint color = 0, byte width = 0) {
			if (Game.main == null) {
				throw new Exception("Cannot draw lines before creating a game");
			}
			if (Instance == null) {
				Instance = new Gizmos();
			}
			if (color == 0) {
				color = defaultColor;
			}
			if (width == 0) {
				width = defaultWidth;
			}

			if (space == null) {
				Instance.drawCalls.Add(new DrawLineCall(x1, y1, x2, y2, color, width));
			} else {
				// transform to the given parent space:
				Vector2 start = space.TransformPoint(x1, y1);
				Vector2 end = space.TransformPoint(x2, y2);

				Instance.drawCalls.Add(new DrawLineCall(start.x, start.y, end.x, end.y, color, width));
			}
		}

		/// <summary>
		/// Draws a plus shape centered at the point x,y, with given radius, using DrawLine.
		/// </summary>
		public static void DrawPlus(float x, float y, float radius, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(x - radius, y, x + radius, y, space, color, width);
			DrawLine(x, y - radius, x, y + radius, space, color, width);
		}

		/// <summary>
		/// Draws a cross shape centered at the point x,y, with given radius, using DrawLine.
		/// </summary>
		public static void DrawCross(float x, float y, float radius, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(x - radius, y - radius, x + radius, y + radius, space, color, width);
			DrawLine(x - radius, y + radius, x + radius, y - radius, space, color, width);
		}

		/// <summary>
		/// Draws a line segment from (x,y) to (x+dx, y+dy), using DrawLine.
		/// </summary>
		public static void DrawRay(float x, float y, float dx, float dy, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(x, y, x + dx, y + dy, space, color, width);
		}

		/// <summary>
		/// Draws a line segment starting at (x,y), with the given length and angle in degrees,
		/// using DrawLine.
		/// </summary>
		public static void DrawRayAngle(float x, float y, float angleDegrees, float length, GameObject space = null, uint color = 0, byte width = 0) {
			float dx = Mathf.Cos(angleDegrees * Mathf.PI / 180) * length;
			float dy = Mathf.Sin(angleDegrees * Mathf.PI / 180) * length;
			DrawLine(x, y, x + dx, y + dy, space, color, width);
		}

		/// <summary>
		/// Draws an arrow from (x,y) to (x+dx, y+dy), using DrawLine.
		/// The relativeArrowSize is the size of the arrow head compared to the arrow length.
		/// </summary>
		public static void DrawArrow(float x, float y, float dx, float dy, float relativeArrowSize = 0.25f, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(x, y, x + dx, y + dy, space, color, width);
			DrawLine(x + dx, y + dy, x + dx * (1 - relativeArrowSize) - dy * relativeArrowSize, y + dy * (1 - relativeArrowSize) + dx * relativeArrowSize, space, color, width);
			DrawLine(x + dx, y + dy, x + dx * (1 - relativeArrowSize) + dy * relativeArrowSize, y + dy * (1 - relativeArrowSize) - dx * relativeArrowSize, space, color, width);
		}

		/// <summary>
		/// Draws an arrow starting at (x,y), with the given length and angle in degrees,
		/// using DrawLine.
		/// The relativeArrowSize is the size of the arrow head compared to the arrow length.
		/// </summary>
		public static void DrawArrowAngle(float x, float y, float angleDegrees, float length, float relativeArrowSize = 0.25f, GameObject space = null, uint color = 0, byte width = 0) {
			float dx = Mathf.Cos(angleDegrees * Mathf.PI / 180) * length;
			float dy = Mathf.Sin(angleDegrees * Mathf.PI / 180) * length;
			DrawArrow(x, y, dx, dy, relativeArrowSize, space, color, width);
		}

		/// <summary>
		/// Draws an axis-aligned rectangle centered at a given point, with given width and height,
		/// using DrawLine.
		/// </summary>
		public static void DrawRectangle(float xCenter, float yCenter, float width, float height, GameObject space = null, uint color = 0, byte lineWidth = 0) {
			DrawLine(xCenter - width / 2, yCenter - height / 2, xCenter + width / 2, yCenter - height / 2, space, color, lineWidth);
			DrawLine(xCenter - width / 2, yCenter + height / 2, xCenter + width / 2, yCenter + height / 2, space, color, lineWidth);
			DrawLine(xCenter - width / 2, yCenter - height / 2, xCenter - width / 2, yCenter + height / 2, space, color, lineWidth);
			DrawLine(xCenter + width / 2, yCenter - height / 2, xCenter + width / 2, yCenter + height / 2, space, color, lineWidth);
		}

		/// <summary>
		/// This method should typically be called from the RenderSelf method of a GameObject,
		/// or from the game's OnAfterRender event.
		/// The line from (x1,y1) to (x2,y2) is then drawn immediately, 
		/// behind objects that are drawn later.
		/// It is drawn in the space of the game object itself if called from RenderSelf with 
		/// pGlobalCoords=false, and in screen space otherwise.
		/// You can give color and line width. If no values are given (=0), the default values are
		/// used. These can be set using SetStyle, SetColor and SetWidth.
		/// </summary>
		public static void RenderLine(float x1, float y1, float x2, float y2, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false) {
			if (pGlobalCoords) GL.LoadIdentity();
			GL.Disable(GL.TEXTURE_2D);
			GL.LineWidth(pLineWidth);
			GL.Color4ub((byte)((pColor >> 16) & 0xff), (byte)((pColor >> 8) & 0xff), (byte)((pColor) & 0xff), (byte)((pColor >> 24) & 0xff));
			float[] vertices = new float[] { x1, y1, x2, y2 };
			GL.EnableClientState(GL.VERTEX_ARRAY);
			GL.VertexPointer(2, GL.FLOAT, 0, vertices);
			GL.DrawArrays(GL.LINES, 0, 2);
			GL.DisableClientState(GL.VERTEX_ARRAY);
			GL.Enable(GL.TEXTURE_2D);
		}

		void DrawLines(GLContext glContext) {
			if (drawCalls.Count > 0) {
				foreach (var dc in drawCalls) {
					RenderLine(dc.x1, dc.y1, dc.x2, dc.y2, dc.color, dc.width, true);
				}
				drawCalls.Clear();
			}
		}
	}
}