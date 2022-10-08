using GXPEngine.GXPEngine.Core;

namespace GXPEngine.GXPEngine;

public class UVOffsetSprite : Sprite
{
	private float[] _vertices;

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
		_vertices = verts;
	}

	// private void DrawWarpedDecal(Vector2[] pos)
	// {
	// 	Vector2 center;
	// 	float rd = ((pos[2].x - pos[0].x) * (pos[3].y - pos[1].y) - (pos[3].x - pos[1].x) * (pos[2].y - pos[0].y));
	// 	if (rd != 0)
	// 	{
	// 		rd = 1.0f / rd;
	// 		float rn = ((pos[3].x - pos[1].x) * (pos[0].y - pos[1].y) - (pos[3].y - pos[1].y) * (pos[0].x - pos[1].x)) * rd;
	// 		float sn = ((pos[2].x - pos[0].x) * (pos[0].y - pos[1].y) - (pos[2].y - pos[0].y) * (pos[0].x - pos[1].x)) * rd;
	// 		if (!(rn < 0.f || rn > 1.f || sn < 0.f || sn > 1.f)) center = pos[0] + rn * (pos[2] - pos[0]);
	// 	}
	// }

	protected override void setUVs()
	{
		float left = _mirrorX ? 1.0f : 0.0f;
		float right = _mirrorX ? 0.0f : 1.0f;
		float top = _mirrorY ? 1.0f : 0.0f;
		float bottom = _mirrorY ? 0.0f : 1.0f;
		_uvs = new float[8] { left, top, right, top, right, bottom, left, bottom };
	}

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

	protected override void RenderSelf(GLContext glContext)
	{
		FillSkewedSquare(glContext);
	}

	private void FillSkewedSquare(GLContext glContext)
	{
		texture.wrap = true;
		blendMode?.enable();
		_texture.Bind();
		(byte r, byte g, byte b, byte a) tempColor  = GetColor();
		glContext.SetColor(tempColor.r, tempColor.g, tempColor.b, tempColor.a);
		glContext.DrawQuad(_vertices, _uvs);
		_texture.Unbind();
		if (blendMode != null) BlendMode.NORMAL.enable();
	}
}
