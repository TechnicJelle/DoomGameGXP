//#define USE_FMOD_AUDIO

using System;
using System.Threading;
using System.Collections.Generic;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// The Sound Class represents a Sound resource in memory
	/// You can load .mp3, .ogg or .wav
	/// </summary>
	public class Sound
	{
        private static Dictionary<string, IntPtr> _soundCache = new Dictionary<string, IntPtr>();

        private IntPtr _id;
        private SoundSystem _system;

        /// <summary>
        /// Creates a new <see cref="GXPEngine.Sound"/>.
        /// This class represents a sound file.
        /// Sound files are loaded into memory unless you set them to 'streamed'.
        /// An optional parameter allows you to create a looping sound.
        /// </summary>
        /// <param name='filename'>
        /// Filename, should include path and extension.
        /// </param>
        /// <param name='looping'>
        /// If set to <c>true</c> the sound file repeats itself. (It loops)
        /// </param>
        /// <param name='streaming'>
        /// If set to <c>true</c>, the file will be streamed rather than loaded into memory.
        /// </param>
        /// <param name='cached'>
        /// If set to <c>true</c>, the sound will be stored in cache, preserving memory when creating the same sound multiple times.
        /// </param>
        public Sound( String filename, bool looping = false, bool streaming = false)
		{
            _system = GLContext.soundSystem;

            if (streaming) {
                _id = _system.CreateStream(filename, looping);
			} else {
                if (!_soundCache.ContainsKey(filename))
                {
                    _id = _system.LoadSound(filename, looping);
                    if (_id == IntPtr.Zero)
                    {
                        throw new Exception("Sound file not found: " + filename);
                    }
                    _soundCache[filename] = _id;
                }
                else
                {
                    _id = _soundCache[filename];
                }
			}
		}
		
		~Sound()
		{
		}

		/// <summary>
		/// Play the specified paused and return the newly created SoundChannel
		/// </summary>
		/// <param name='paused'>
		/// When set to <c>true</c>, the sound is set up, but remains paused.
		/// You can use this to set frequency, panning and volume before playing the sound.
		/// </param>
		/// <param name='channelId'>
		/// When in range 0...31, the selected channel will be used. If it already
		/// contains a playing sound, that sound will be stopped.
		/// When set to -1 (the default), the next free channel will be used.
		/// However, when all channels are in use, Sound.Play will silently fail.
		/// </param>
		public SoundChannel Play( bool paused = false, uint channelId=0, float volume=1, float pan=0)
		{

			#if !USE_FMOD_AUDIO
			if (channelId != 0)
			{
				throw new Exception("Channel ID is not supported when using SoLoud audio. Please change #define in GLContext.cs!");
			}
			#else
			if (channelId==0) {
			channelId=4294967295; // -1 basically (since FMOD actually works with ints), which means: pick a free channel.
			}
			#endif
			uint channelID = _system.PlaySound(_id, channelId, paused, volume, pan);
			SoundChannel soundChannel = new SoundChannel( channelID );
			return soundChannel;
		}
	}
}
