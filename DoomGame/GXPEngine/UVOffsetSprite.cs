﻿using System.Collections.Generic;
using GXPEngine.Core;
using GXPEngine;

public class UVOffsetSprite : Sprite
{
	public byte brightness = 255;

	private float offsetX;
	private float offsetY;

	private Vector2 tl;
	private Vector2 tr;
	private Vector2 bl;
	private Vector2 br;

	public UVOffsetSprite(string filename, IReadOnlyList<Vector2> corners) : base(filename)
	{
		texture.wrap = true;
		tl = new Vector2(corners[0].x, corners[0].y); //top-left
		tr = new Vector2(corners[1].x, corners[1].y); //top-right
		bl = new Vector2(corners[2].x, corners[2].y); //bottom-left
		br = new Vector2(corners[3].x, corners[3].y); //bottom-right
	}

	public void SetCorners(Vector2[] corners)
	{
		tl = new Vector2(corners[0].x, corners[0].y); //top-left
		tr = new Vector2(corners[1].x, corners[1].y); //top-right
		bl = new Vector2(corners[2].x, corners[2].y); //bottom-left
		br = new Vector2(corners[3].x, corners[3].y); //bottom-right
	}

	public void SetOffset(float x, float y)
	{
		offsetX = x;
		offsetY = y;

		setUVs();
	}

	public void AddOffset(float x, float y)
	{
		offsetX += x;
		offsetY += y;

		setUVs();
	}

	protected override void setUVs()
	{
		float left = _mirrorX ? 1.0f : 0.0f;
		float right = _mirrorX ? 0.0f : 1.0f;
		float top = _mirrorY ? 1.0f : 0.0f;
		float bottom = _mirrorY ? 0.0f : 1.0f;

		left += offsetX;
		right += offsetX;
		top += offsetY;
		bottom += offsetY;
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
		glContext.SetColor(brightness, brightness, brightness, 255);
		float[] area = {
			tl.x, tl.y,
			tr.x, tr.y,
			br.x, br.y,
			bl.x, bl.y
		};
		glContext.DrawQuad(area, _uvs);
		_texture.Unbind();
		if (blendMode != null) BlendMode.NORMAL.enable();
	}
}