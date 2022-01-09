using System;

namespace GXPEngine.Core
{
	public struct Rectangle
	{
		public float x, y, width, height;
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Rectangle()
		//------------------------------------------------------------------------------------------------------------------------
		public Rectangle (float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Properties()
		//------------------------------------------------------------------------------------------------------------------------
		public float left { get { return x; } }
		public float right { get { return x + width; } }
		public float top { get { return y; } }
		public float bottom { get { return y + height; } }

		//------------------------------------------------------------------------------------------------------------------------
		//														ToString()
		//------------------------------------------------------------------------------------------------------------------------
		override public string ToString() {
			return (x + "," + y + "," + width + "," + height);
		}
		
	}
}

