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

namespace GXPEngine;

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
	private const string SETTINGS_FILE_NAME = "settings.txt"; // should be in bin/Debug or bin/Release. Use "MySubFolder/settings.txt" for subfolders.
	private const bool SHOW_SETTINGS_PARSING = false; // If true, settings parsing progress is printed to console
	private const bool THROW_EXCEPTION_ON_MISSING_SETTING = true;

	//Gameplay
	public static bool Minimap = true;

	// Screen
	public static bool FullScreen = false;
	public static bool AntiAliasing = false;
	public static float FieldOfViewDegrees = 10.0f;
	public static int Width = 1280;
	public static int Height = 720;
	public static int ScreenResolutionX = 1280;
	public static int ScreenResolutionY = 720;

	//Controls
	public static int Up = Key.W;
	public static int Left = Key.A;
	public static int Down = Key.S;
	public static int Right = Key.D;

	public static bool DebugMode = false;

	/// <summary>
	/// Load new values from the file settings.txt
	/// </summary>
	public static void Load() {
		ReadSettingsFromFile();
	}

	private static void Warn(string pWarning, bool alwaysContinue = false) {
		string message = "Settings.cs: " + pWarning;
		if (THROW_EXCEPTION_ON_MISSING_SETTING && !alwaysContinue)
			throw new Exception(message);
		else
			Console.WriteLine("WARNING: "+message);
	}

	private static void ReadSettingsFromFile()
	{
		if (SHOW_SETTINGS_PARSING) Console.WriteLine("Reading settings from file");

		if (!File.Exists(SETTINGS_FILE_NAME)) {
			Warn("No settings file found");
			return;
		}

		StreamReader reader = new StreamReader(SETTINGS_FILE_NAME);

		string line = reader.ReadLine();
		while (line != null)
		{
			if (line.Length > 0 && line.Substring(0, 2) != "//")
			{
				if (SHOW_SETTINGS_PARSING) Console.WriteLine("Read a non-comment line: " + line);
				string[] words = line.Split('=');
				if (words.Length == 2)
				{
					// Remove all white space characters at start and end (but not in between non-white space characters):
					words[0] = words[0].Trim();
					words[1] = words[1].Trim();

					object value;
					// InvariantCulture is necessary to override (e.g. Dutch) locale settings when using .NET: the decimal separator is a dot, not a comma.
					if (int.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out int intValue))
					{
						value = intValue;
						if (SHOW_SETTINGS_PARSING) Console.WriteLine(" integer argument: Key {0} Value {1}",words[0],value);
					}
					else if (float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
						         out float floatValue))
					{
						value = floatValue;
						if (SHOW_SETTINGS_PARSING) Console.WriteLine(" float argument: Key {0} Value {1}",words[0],value);
					}
					else if (bool.TryParse(words[1], out bool boolValue))
					{
						value = boolValue;
						if (SHOW_SETTINGS_PARSING) Console.WriteLine(" boolean argument: Key {0} Value {1}",words[0],value);
					}
					else
					{
						value = words [1];
						if (SHOW_SETTINGS_PARSING) Console.WriteLine(" string argument: Key {0} Value {1}",words[0],words[1]);
					}

					FieldInfo field = typeof(Settings).GetField(words[0],BindingFlags.Static | BindingFlags.Public);
					if (field!=null) {
						try {
							// unpacking happens here:
							// (If you want to load more than the supported types, such as Sounds, add your own
							//  logic here, similar to the code below for the string[] type:)
							if (field.FieldType == typeof(string[])) {
								string[] valueWords=words[1].Split(',');
								for (int i=0;i<valueWords.Length;i++) {
									valueWords[i]=valueWords[i].Trim();
								}
								field.SetValue(null,valueWords);
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
				if (SHOW_SETTINGS_PARSING) Console.WriteLine("Comment line or empty line: " + line);
			}
			line = reader.ReadLine();
		}
		reader.Close();
	}
}