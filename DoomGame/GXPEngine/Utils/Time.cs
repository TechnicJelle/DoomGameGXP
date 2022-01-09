using System.Diagnostics;
using System;

namespace GXPEngine
{
	/// <summary>
	/// Contains various time related functions.
	/// </summary>
	public class Time
	{
		private static int previousTime;

		static Time() {
		}
		
		/// <summary>
		/// Returns the current system time in milliseconds
		/// </summary>
		public static int now {
			get { return System.Environment.TickCount; }
		}
		
		/// <summary>
		/// Returns this time in milliseconds since the program started		
		/// </summary>
		/// <value>
		/// The time.
		/// </value>
		public static int time {
			get { return (int)(OpenGL.GL.glfwGetTime()*1000); }
		}
		
		/// <summary>
		/// Returns the time in milliseconds that has passed since the previous frame
		/// </summary>
		/// <value>
		/// The delta time.
		/// </value>
		private static int previousFrameTime;
		public static int deltaTime {
			get { 
				return previousFrameTime; 
			}
		}

		internal static void newFrame() {
			previousFrameTime = time - previousTime;
			previousTime = time;
		}
	}
}

