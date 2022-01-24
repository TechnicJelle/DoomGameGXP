using GXPEngine.Core;
using GXPEngine;

public class UVOffsetSprite : Sprite
{
	private float[] vertices;

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
		vertices = verts;
	}

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
		glContext.DrawQuad(vertices, _uvs);
		_texture.Unbind();
		if (blendMode != null) BlendMode.NORMAL.enable();
	}
}