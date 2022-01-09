using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

/* Here is an example of a possible settings file (create a file settings.txt in bin/Debug, and copy the next lines to it):


// You can add comment lines like this to settings.txt,
//  but only if the line *starts with* two forward slashes.
// Make sure the capitalization and spelling of variable names matches those in Settings.cs.
// (No semi colon is needed at the end of a line.)
// integer values:
Width = 800
Height = 600
// boolean values:
FullScreen = true
// string values (no quotes needed):
SettingsFileName = settings.txt
// Currently Settings.cs contains no float parameters or string arrays, but these would be 
// initialized as follows (no f needed):
// MyFloat=1.5
// MyLevels = Level1.tmx, Level2.tmx, Level3.tmx 


*/

namespace GXPEngine {
	/// <summary>
	/// Static class that contains various settings, such as screen resolution and player controls. 
	/// In your Main method, you can Call the Settings.Load() method to initialize these settings from a text file 
	/// (typically settings.txt, which should be present in the bin/Debug and/or bin/Release folder).
	/// 
	/// The purpose is that you can easily change certain settings this way, without recompiling the code, 
	/// and that during development different people can use different settings while working with the same build.
	/// 
	/// Feel free to add all kinds of other useful properties to this class. They will be initialized from the text file as long
	/// as they are of one of the following types:
	///   public static int
	///   public static float
	///   public static bool
	///   public static string
	///   public static string[]
	/// </summary>
	public class Settings
	{
		// Settings that are related to this class and the parsing process:
		public static string SettingsFileName = "settings.txt"; // should be in bin/Debug or bin/Release. Use "MySubFolder/settings.txt" for subfolders.
		public static bool ShowSettingsParsing = false;  // If true, settings parsing progress is printed to console
		public static bool ThrowExceptionOnMissingSetting = true; 

		// Resolution values - use these when creating a new MyGame instance:
		// (Note: for the arcade machine, use ScreenResolutionX,Y = 1600,1200)
		public static int Width = 800;
		public static int Height = 600;
		public static int ScreenResolutionX = 800;
		public static int ScreenResolutionY = 600;
		public static bool FullScreen = false;

		// In your Player class, you can use these keys for player controls:
		// (Default: arcade machine setup:)
		// PLAYER 1:
		// WASD:
		public static int P1Up=87;
		public static int P1Left=65;
		public static int P1Down=83;
		public static int P1Right=68;
		// FGHVBN:
		public static int P1Fire1=70;
		public static int P1Fire2=71;
		public static int P1Fire3=72;
		public static int P1Fire4=86;
		public static int P1Fire5=66;
		public static int P1Fire6=78;

		// PLAYER 2:
		// IJKL:
		public static int P2Up=73;
		public static int P2Left=74;
		public static int P2Down=75;
		public static int P2Right=76;
		// numpad 1-6:
		public static int P2Fire1=306;
		public static int P2Fire2=307;
		public static int P2Fire3=308;
		public static int P2Fire4=303;
		public static int P2Fire5=304;
		public static int P2Fire6=305;

		// GENERAL BUTTONS:
		// one,two:
		public static int Start1P=49;
		public static int Start2P=50;
		// enter:
		public static int Menu=294;

		/// <summary>
		/// Load new values from the file settings.txt
		/// </summary>
		public static void Load() {
			ReadSettingsFromFile();
		}

		private static void Warn(string pWarning, bool alwaysContinue = false) {
			string message = "Settings.cs: " + pWarning;
			if (ThrowExceptionOnMissingSetting && !alwaysContinue)
				throw new Exception(message);
			else
				Console.WriteLine("WARNING: "+message);
		}

		private static void ReadSettingsFromFile()
		{
			if (ShowSettingsParsing) Console.WriteLine("Reading settings from file");

			if (!File.Exists(SettingsFileName)) {
	            Warn("No settings file found");
				return;
			}

			StreamReader reader = new StreamReader(SettingsFileName);

			string line = reader.ReadLine();
			while (line != null)
			{
				if (line.Length < 2 || line.Substring(0, 2) != "//")
				{
					if (ShowSettingsParsing) Console.WriteLine("Read a non-comment line: " + line);
					string[] words = line.Split('='); 
					if (words.Length == 2)
					{
						// Remove all white space characters at start and end (but not in between non-white space characters):
						words[0] = words[0].Trim();
						words[1] = words[1].Trim();

						Object value;
						bool boolValue;
						float floatValue;
						int intValue;
						// InvariantCulture is necessary to override (e.g. Dutch) locale settings when using .NET: the decimal separator is a dot, not a comma.
						if (int.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out intValue))					
						{
							value = intValue;
							if (ShowSettingsParsing) Console.WriteLine(" integer argument: Key {0} Value {1}",words[0],value);
						}
						else if (float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
												out floatValue))
						{
							value = floatValue;
							if (ShowSettingsParsing) Console.WriteLine(" float argument: Key {0} Value {1}",words[0],value);
						}
						else if (bool.TryParse(words[1], out boolValue))
						{
							value = boolValue;
							if (ShowSettingsParsing) Console.WriteLine(" boolean argument: Key {0} Value {1}",words[0],value);
						}
						else
						{
							value = words [1];
							if (ShowSettingsParsing) Console.WriteLine(" string argument: Key {0} Value {1}",words[0],words[1]);
						}

						FieldInfo field = typeof(Settings).GetField(words[0],BindingFlags.Static | BindingFlags.Public);
						if (field!=null) {
							try {
								// unpacking happens here:
								// (If you want to load more than the supported types, such as Sounds, add your own
								//  logic here, similar to the code below for the string[] type:)
								if (field.FieldType == typeof(string[])) {
									string[] valuewords=words[1].Split(',');
									for (int i=0;i<valuewords.Length;i++) {
										valuewords[i]=valuewords[i].Trim();
									}
									field.SetValue(null,valuewords);
								} else {
									field.SetValue (null, value);
								}
							} catch (Exception error) {
								Warn ("Cannot set field "+words[0]+": type mismatch? "+error.Message);
							}
						}else {
							Warn ("No field with name "+words[0]+" exists!");
						}
					}
					else
					{
						Warn("Malformed line (expected one '=' character): "+line);
					}
				}
				else
				{
					//if (ShowSettingsParsing) Console.WriteLine("Comment line or empty line: " + line);
				}
				line = reader.ReadLine();
			}
			reader.Close();
		}
	}
}