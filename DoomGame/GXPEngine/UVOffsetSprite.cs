using System.Collections.Generic;
using GXPEngine.GXPEngine.Core;

namespace GXPEngine.GXPEngine;

public class UVOffsetSprite : Sprite
{
	private Vector2[] _vertices;
	private new Vector2[] _uvs;
	private float[] _ws;

	public UVOffsetSprite(string filename, bool keepInCache=false, bool addCollider=true) : base(filename, keepInCache, addCollider)
	{
		SetVertices(new [] {0.0f, 0.0f, 100.0f, 0.0f, 100.0f, 100.0f, 0.0f, 100.0f});
		texture.wrap = true;
	}

	/// <param name="verts">
	/// (left, top), (right, top), (right, bottom), (left, bottom)
	/// </param>
	public void SetVertices(float[] verts)
	{
		DrawWarpedDecal(new []
		{
				new Vector2(verts[0], verts[1]),
				new Vector2(verts[6], verts[7]),
				new Vector2(verts[4], verts[5]),
				new Vector2(verts[2], verts[3]),
		});
	}

	private void DrawWarpedDecal(IList<Vector2> pos)
	{
		//Method to update the screen positions and UVs etc of the warped sprite after which it'll get rendered by the engine in the methods below this one
		//Inspiration: https://github.com/OneLoneCoder/olcPixelGameEngine/blob/ccedd4ecf993cd882b92846cb88a7ff8630bc150/olcPixelGameEngine.h#L2542
		float[] diW = {1, 1, 1, 1};
		Vector2[] diPos = new Vector2[4];
		Vector2[] diUV = {
			new(0.0f, 0.0f),
			new(0.0f, 1.0f),
			new(1.0f, 1.0f),
			new(1.0f, 0.0f),
		};

		Vector2 center = new();
		float rd = ((pos[2].x - pos[0].x) * (pos[3].y - pos[1].y) - (pos[3].x - pos[1].x) * (pos[2].y - pos[0].y));
		if (rd != 0)
		{
			rd = 1.0f / rd;
			float rn = ((pos[3].x - pos[1].x) * (pos[0].y - pos[1].y) - (pos[3].y - pos[1].y) * (pos[0].x - pos[1].x)) * rd;
			float sn = ((pos[2].x - pos[0].x) * (pos[0].y - pos[1].y) - (pos[2].y - pos[0].y) * (pos[0].x - pos[1].x)) * rd;
			if (!(rn is < 0.0f or > 1.0f || sn is < 0.0f or > 1.0f)) center = pos[0] + rn * (pos[2] - pos[0]);
			float[] d = new float[4];
			for (int i = 0; i < 4; i++) d[i] = (pos[i] - center).Mag();
			for (int i = 0; i < 4; i++)
			{
				float q = d[i] == 0.0f ? 1.0f : (d[i] + d[(i + 2) & 3]) / d[(i + 2) & 3];
				diUV[i] *= q;
				diW[i] *= q;
				diPos[i] = new Vector2(pos[i].x , pos[i].y);
			}
		}

		_uvs = diUV;
		_ws = diW;
		_vertices = diPos;
	}

	public override void Render(GLContext glContext)
	{
		if (!visible) return;
		glContext.PushMatrix(matrix);

		RenderSelf(glContext);
		foreach (GameObject child in GetChildren())
		{
			child.Render(glContext);
		}

		glContext.PopMatrix();
	}

	protected override void RenderSelf(GLContext glContext)
	{
		FillSkewedSquare(glContext);
	}

	private void FillSkewedSquare(GLContext glContext)
	{
		texture.wrap = true;
		blendMode?.enable();
		_texture.Bind();
		glContext.DrawWarpedQuad(_uvs, _vertices, _ws, 4, GetColor());
		_texture.Unbind();
		if (blendMode != null) BlendMode.NORMAL.enable();
	}
}
