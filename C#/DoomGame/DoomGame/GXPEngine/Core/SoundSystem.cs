using System;
using System.Collections.Generic;

namespace GXPEngine.Core
{
    public abstract class SoundSystem
    {
        public abstract void Init();
        public abstract void Deinit();
        public abstract IntPtr CreateStream(string filename, bool looping);
        public abstract IntPtr LoadSound(string filename, bool looping);
        public abstract void Step();
        public abstract uint PlaySound(IntPtr id, uint channelId, bool paused);
		public abstract uint PlaySound (IntPtr id, uint channelId, bool paused, float volume, float pan);

        public abstract float GetChannelFrequency(uint channelId);
        public abstract void SetChannelFrequency(uint channelId, float frequency);
        public abstract float GetChannelPan(uint channelId);
        public abstract void SetChannelPan(uint channelId, float pan);
        public abstract float GetChannelVolume(uint channelId);
        public abstract void SetChannelVolume(uint channelId, float volume);
        public abstract bool GetChannelPaused(uint channelId);
        public abstract void SetChannelPaused(uint channelId, bool pause);
        public abstract bool ChannelIsPlaying(uint channelId);
        public abstract void StopChannel(uint channelId);
    }
}