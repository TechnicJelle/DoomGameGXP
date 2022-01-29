using System.Collections.Generic;
using GXPEngine.Core;
using GXPEngine;
using GXPEngine.MyGame;

public class WarpedSprite : Sprite
{
	//Inspiration: https://github.com/OneLoneCoder/olcPixelGameEngine/blob/ccedd4ecf993cd882b92846cb88a7ff8630bc150/olcPixelGameEngine.h#L807
	private int di_points = 0;
	private List<Vector2> di_pos;
	private List<Vector2> di_uv;
	private List<float> di_w;
	//tint is not needed, because Sprite already has a color

	public WarpedSprite(string filename, bool keepInCache=false, bool addCollider=false) : base(filename, keepInCache, addCollider)
	{
		//Set up the properties with some useless variables, just to make sure they're instantiated
		//This method will get called with the correct inputs before this warped sprite is drawn to screen anyway.
		SetPoints(new List<Vector2> {new(0.0f, 0.0f), new(0.0f, 100.0f), new(100.0f, 100.0f), new(100.0f, 0.0f)});
		texture.wrap = true;
	}

	public void SetPoints(List<Vector2> pos)
	{
		//Method to update the screen positions and UVs etc of the warped sprite after which it'll get rendered by the engine in the methods below this one
		//Inspiration: https://github.com/OneLoneCoder/olcPixelGameEngine/blob/ccedd4ecf993cd882b92846cb88a7ff8630bc150/olcPixelGameEngine.h#L2542
		di_points = 4;
		di_w = new List<float> {1, 1, 1, 1};
		di_uv = new List<Vector2> {new(0.0f, 0.0f), new(0.0f, 1.0f), new(1.0f, 1.0f), new(1.0f, 0.0f)};
		di_pos = new List<Vector2> {new(0.0f, 0.0f), new(0.0f, 100.0f), new(100.0f, 100.0f), new(100.0f, 0.0f)};
		Vector2 center = new();
		float rd = (pos[2].x - pos[0].x) * (pos[3].y - pos[1].y) - (pos[3].x - pos[1].x) * (pos[2].y - pos[0].y);
		if (rd != 0)
		{
			rd = 1.0f / rd;
			float rn = ((pos[3].x - pos[1].x) * (pos[0].y - pos[1].y) - (pos[3].y - pos[1].y) * (pos[0].x - pos[1].x)) * rd;
			float sn = ((pos[2].x - pos[0].x) * (pos[0].y - pos[1].y) - (pos[2].y - pos[0].y) * (pos[0].x - pos[1].x)) * rd;
			if (!(rn is < 0.0f or > 1.0f || sn is < 0.0f or > 1.0f)) center = pos[0].Add(Vector2.Sub(pos[2],pos[0]).Mult(rn));
			float[] d = new float[4];
			for (int i = 0; i < 4; i++) d[i] = Vector2.Sub(pos[i], center).Mag();
			for (int i = 0; i < 4; i++)
			{
				float q = d[i] == 0.0f ? 1.0f : (d[i] + d[(i + 2) & 3]) / d[(i + 2) & 3];
				di_uv[i].Mult(q);
				di_w[i] *= q;
				di_pos[i] = new Vector2(
					(pos[i].x * 1.0f / MyGame.staticWidth) * 2.0f - 1.0f,
					(pos[i].y * 1.0f / MyGame.staticHeight * 2.0f - 1.0f) * -1.0f
				);
			}
		}
	}

	//Called by the engine
	public override void Render(GLContext glContext)
	{
		if (!visible) return;
		glContext.PushMatrix(matrix);

		RenderSelf (glContext);
		foreach (GameObject child in GetChildren()) {
			child.Render(glContext);
		}

		glContext.PopMatrix();
	}

	//Called by the engine
	protected override void RenderSelf(GLContext glContext)
	{
		FillSkewedSquare(glContext);
	}

	//Called by the engine
	private void FillSkewedSquare(GLContext glContext)
	{
		texture.wrap = true;
		blendMode?.enable();
		_texture.Bind();
		glContext.DrawWarpedQuad(di_uv, di_pos, di_w, di_points, GetColor());
		_texture.Unbind();
		if (blendMode != null) BlendMode.NORMAL.enable();
	}
}