using System;
using System.Security;
using System.Runtime.InteropServices;

namespace GXPEngine.Core
{
	public class FMOD
	{
  		public const int FMOD_DEFAULT = 0x00000000;
  		public const int FMOD_LOOP_OFF = 0x00000001;
		public const int FMOD_LOOP_NORMAL = 0x00000002;
  		public const int FMOD_LOOP_BIDI = 0x00000004;
  		public const int FMOD_2D = 0x00000008;
		public const int FMOD_3D = 0x00000010;
  		public const int FMOD_HARDWARE = 0x00000020;
  		public const int FMOD_SOFTWARE = 0x00000040;
  		public const int FMOD_CREATESTREAM = 0x00000080;
  		public const int FMOD_CREATESAMPLE = 0x00000100;
  		public const int FMOD_CREATECOMPRESSEDSAMPLE = 0x00000200;
  		public const int FMOD_OPENUSER = 0x00000400;
  		public const int FMOD_OPENMEMORY = 0x00000800;
  		public const int FMOD_OPENMEMORY_POINT = 0x10000000;
  		public const int FMOD_OPENRAW = 0x00001000;
  		public const int FMOD_OPENONLY = 0x00002000;
  		public const int FMOD_ACCURATETIME = 0x00004000;
  		public const int FMOD_MPEGSEARCH = 0x00008000;
  		public const int FMOD_NONBLOCKING = 0x00010000;
  		public const int FMOD_UNIQUE = 0x00020000;
		public const int FMOD_3D_HEADRELATIVE = 0x00040000;
  		public const int FMOD_3D_WORLDRELATIVE = 0x00080000;
  		public const int FMOD_3D_INVERSEROLLOFF = 0x00100000;
  		public const int FMOD_3D_LINEARROLLOFF = 0x00200000;
  		public const int FMOD_3D_LINEARSQUAREROLLOFF = 0x00400000;
  		public const int FMOD_3D_CUSTOMROLLOFF = 0x04000000;
  		public const int FMOD_3D_IGNOREGEOMETRY = 0x40000000;
  		public const int FMOD_UNICODE = 0x01000000;
  		public const int FMOD_IGNORETAGS = 0x02000000;
  		public const int FMOD_LOWMEM = 0x08000000;
  		public const int FMOD_LOADSECONDARYRAM = 0x20000000;
  		public const uint FMOD_VIRTUAL_PLAYFROMSTART = 0x80000000;	
		public const int FMOD_CHANNEL_FREE = -1;
		public const int FMOD_CHANNEL_REUSE = -2;

// System		
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_System_Create")]
		public static extern void System_Create(out IntPtr system);
		
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_System_Init")]
		public static extern void System_Init(IntPtr system, int maxChannels, uint flags, int extraDriverData);
		
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_System_CreateSound")]
		public static extern void System_CreateSound(IntPtr system, string filename, uint mode, int uk, out IntPtr sound);

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_System_CreateStream")]
		public static extern void System_CreateStream(IntPtr system, string filename, uint mode, int uk, out IntPtr sound);
		
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_System_PlaySound")]
		public static extern int System_PlaySound(IntPtr system, uint channelpref, IntPtr sound, bool paused, out uint channel);

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_System_Update")]
		public static extern void System_Update(IntPtr system);

// Channel
	// Frequency
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_GetFrequency")]
		public static extern void Channel_GetFrequency( uint channel, out float frequency );

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_SetFrequency")]
		public static extern void Channel_SetFrequency( uint channel, float frequency );
		
	// Stop
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_Stop")]
		public static extern void Channel_Stop( uint channel );

	// Mute
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_GetMute")]
		public static extern void Channel_GetMute( uint channel, out bool mute );

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_SetMute")]
		public static extern void Channel_SetMute( uint channel, bool mute );

	// Pan
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_GetPan")]
		public static extern void Channel_GetPan( uint channel, out float pan );

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_SetPan")]
		public static extern void Channel_SetPan( uint channel, float pan );

	// Paused
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_GetPaused")]
		public static extern void Channel_GetPaused( uint channel, out bool paused );

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_SetPaused")]
		public static extern void Channel_SetPaused( uint channel, bool paused );

	// Playing
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_IsPlaying")]
		public static extern void Channel_IsPlaying( uint channel, out bool playing );

	// Spectrum // can also be done in System for total output, not tested yet
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_GetSpectrum")]
		public static extern void Channel_GetSpectrum( uint channel, float [] spectrumarray, int numvalues, uint channeloffset, int windowtype );

	// Volume
		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_GetVolume")]
		public static extern void Channel_GetVolume( uint channel, out float volume );

		[DllImport("lib/fmodex.dll", EntryPoint="FMOD_Channel_SetVolume")]
		public static extern void Channel_SetVolume( uint channel, float volume );
		
	}
}

