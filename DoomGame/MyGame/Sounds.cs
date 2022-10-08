using GXPEngine.GXPEngine;

namespace GXPEngine.MyGame;

public static class Sounds
{
	public static Sound Music { get; private set; }
	public static Sound ButtonClick { get; private set; }
	public static Sound Elevator { get; private set; }

	public static void LoadAllSounds()
	{
		Music = new Sound("197 - Invocation.ogg", true, true);
		ButtonClick = new Sound("ping.wav");
		Elevator = new Sound("elevator_door.wav");
	}
}
