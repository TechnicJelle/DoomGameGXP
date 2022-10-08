using System;
using GXPEngine.GXPEngine;
using GXPEngine.GXPEngine.AddOns;
using GXPEngine.GXPEngine.Core;
using GXPEngine.GXPEngine.Utils;

#pragma warning disable CS0162

namespace GXPEngine.MyGame;

public class Renderable
{
	private readonly UVOffsetSprite _texture;

	protected Vector2 Normal { get; set; }
	protected Vector2 Position;
	protected Vector2 P1 { get; private set; }
	protected Vector2 P2 { get; private set; }
	public float DistToPlayer { get; private set; }

	protected Renderable(string filename)
	{
		_texture = new UVOffsetSprite(filename, true, false);
		Game.main.AddChild(_texture);

		Position = new Vector2(1, 1);
		Normal = new Vector2(1, 0);

		CalculatePointsInWorldSpace();
	}

	public bool AlignsWithPlayer(float threshold = 0.5f) //TODO: base this threshold on the FOV
	{
		return Normal.Dot(MyGame.CurrentLevel.Player.Heading) < threshold;
	}

	public void SetVisibility(bool visibility)
	{
		_texture.visible = visibility;
	}

	protected void CalculatePointsInWorldSpace()
	{
		P1 = new Vector2(Position.x + Normal.y / 2.0f, Position.y - Normal.x / 2.0f);
		P2 = new Vector2(Position.x - Normal.y / 2.0f, Position.y + Normal.x / 2.0f);
	}

	public virtual void RefreshVisuals()
	{
		Game.main.Remove(_texture);
		if (Settings.DebugMode && Settings.Minimap)
		{
			Minimap.DebugNoStroke();

			//Tile Side - Red Dot
			Minimap.DebugFill(255, 0, 0);
			Minimap.DebugCircle(Position.x, Position.y, 4);

			//Minimap: Blue Dot
			Minimap.DebugFill(0, 0, 255);
			Minimap.DebugCircle(P1.x, P1.y, 4);

			//Minimap: Blue Dot
			Minimap.DebugFill(0, 0, 255);
			Minimap.DebugCircle(P2.x, P2.y, 4);

			//Minimap: Normal
			Minimap.DebugStroke(0, 255, 0);
			Minimap.DebugStrokeWeight(2);
			Minimap.DebugLine(Position.x, Position.y, Position.x + Normal.x*0.4f, Position.y + Normal.y*0.4f);
		}

		(int ix1, float distToP1) = MyGame.WorldToScreen(P1);
		float fCeiling1 = MyGame.StaticHeight / 2.0f - MyGame.StaticHeight /
			(distToP1 * Mathf.Cos((P1 - MyGame.CurrentLevel.Player.Position).Heading() - MyGame.CurrentLevel.Player.Angle));
		float fFloor1 = MyGame.StaticHeight - fCeiling1;

		(int ix2, float distToP2) = MyGame.WorldToScreen(P2);
		float fCeiling2 = MyGame.StaticHeight / 2.0f - MyGame.StaticHeight /
			(distToP2 * Mathf.Cos((P2 - MyGame.CurrentLevel.Player.Position).Heading() -MyGame.CurrentLevel.Player.Angle));
		float fFloor2 = MyGame.StaticHeight - fCeiling2;

		DistToPlayer = (distToP1 + distToP2) / 2.0f;

		//Actually drawing the side
		//Inverse Square(ish) Law:
		const float exp = 1.6f;
		float sq = Mathf.Pow(DistToPlayer, exp);
		float wSq = Mathf.Pow(Player.VIEW_DEPTH, exp);
		byte brightness = Convert.ToByte(Mathf.Round(Mathf.Clamp(Mathf.Map(sq, 0, wSq, 255, 0), 0, 255)));

		//Linear lighting:
		// byte brightness = Convert.ToByte(Mathf.Map(distToPlayer, 0, Player.VIEW_DEPTH, 255, 0));

		_texture.visible = true;
		_texture.SetVertices(new[] {ix1, fCeiling1, ix2, fCeiling2, ix2, fFloor2, ix1, fFloor1});
		_texture.SetColor(brightness, brightness, brightness);
		Game.main.AddChild(_texture);
	}
}
