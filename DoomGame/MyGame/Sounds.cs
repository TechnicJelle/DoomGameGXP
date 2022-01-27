namespace GXPEngine.MyGame
{
	public static class Sounds
	{
		public static Sound music { get; private set; }
		public static Sound buttonClick { get; private set; }
		public static Sound elevator { get; private set; }

		public static void LoadAllSounds()
		{
			music = new Sound("197 - Invocation.ogg", true, true);
			buttonClick = new Sound("ping.wav");
			elevator = new Sound("elevator_door.wav");
		}
	}
}