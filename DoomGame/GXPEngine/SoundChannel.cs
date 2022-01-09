using System;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// This class represents a sound channel on the soundcard.
	/// </summary>
	public class SoundChannel
	{
		private uint _id = 0;
        private SoundSystem _system;
        private float _volume = 1f;
        private bool _isMuted = false;

		public uint ID {
			get {
				return _id;
			}
		}

        public SoundChannel( uint id )
		{
            _system = GLContext.soundSystem;
            _id = id;
        }

        /// <summary>
        /// Gets or sets the channel frequency.
        /// </summary>
        /// <value>
        /// The frequency. Defaults to the sound frequency. (Usually 44100Hz)
        /// </value>
        public float Frequency 
		{
			get 
			{
                float frequency = _system.GetChannelFrequency(_id);
				return frequency;
			}
			set
			{
                _system.SetChannelFrequency(_id, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GXPEngine.SoundChannel"/> is mute.
		/// </summary>
		/// <value>
		/// <c>true</c> if you want to mute the sound
		/// </value>
		public bool Mute   
		{
			get 
			{
				return _isMuted;
			}
			set
			{
                _isMuted = value;
                if (value)
                {
                    _system.SetChannelVolume(_id, 0f);
                }
                else
                {
                    _system.SetChannelVolume(_id, _volume);
                }
            }
		}

		/// <summary>
		/// Gets or sets the pan. Value should be in range -1..0..1, for left..center..right
		/// </summary>
		public float Pan   
		{
			get 
			{
                return _system.GetChannelPan(_id);
			}
			set
			{
                _system.SetChannelPan(_id, value);
			}
		}		

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GXPEngine.Channel"/> is paused.
		/// </summary>
		/// <value>
		/// <c>true</c> if paused; otherwise, <c>false</c>.
		/// </value>
		public bool IsPaused   
		{
			get 
			{
                return _system.GetChannelPaused(_id);
			}
			set
			{
                _system.SetChannelPaused(_id, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GXPEngine.Channel"/> is playing. (readonly)
		/// </summary>
		/// <value>
		/// <c>true</c> if playing; otherwise, <c>false</c>.
		/// </value>
		public bool IsPlaying  
		{
			get 
			{
                return _system.ChannelIsPlaying(_id);
			}
		}		
		
		/// <summary>
		/// Stop the channel.
		/// </summary>
		public void Stop()
		{
            _system.StopChannel(_id);
			_id = 0;
		}
	
		/// <summary>
		/// Gets or sets the volume. Should be in range 0...1
		/// </summary>
		/// <value>
		/// The volume.
		/// </value>
		public float Volume 
		{
			get 
			{
                return _system.GetChannelVolume(_id);
			}
			set
			{
                _volume = value;
                if (!_isMuted)
                {
                    _system.SetChannelVolume(_id, value);
                }
			}
		}
		
	}
}
