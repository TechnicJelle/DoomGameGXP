using GXPEngine.GXPEngine.Core;
using GXPEngine.GXPEngine.Utils;

namespace GXPEngine.MyGame;

public class WallSide : Renderable
{
	public WallSide(string filename, int i, float tileCenterX, float tileCenterY) : base(filename)
	{
		float normalDir = Mathf.HALF_PI * i;
		int x = (int) Mathf.Cos(normalDir);
		int y = (int) Mathf.Sin(normalDir);

		Normal = new Vector2(x, y);
		Position = new Vector2(tileCenterX + x / 2.0f, tileCenterY + y / 2.0f);
		CalculatePointsInWorldSpace();
	}
}
