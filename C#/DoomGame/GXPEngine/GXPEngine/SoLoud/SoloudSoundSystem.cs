using System;
using GXPEngine.Core;

namespace GXPEngine.Core
{
    public class SoloudSoundSystem : SoundSystem
    {
        private IntPtr _device;

        public SoloudSoundSystem()
        {
        }

        public override void Init() {
            _device = Soloud.Soloud_create();
            Soloud.Soloud_init(_device);
        }

        public override void Deinit() {
            if (_device != IntPtr.Zero)
            {
                Soloud.Soloud_stopAll(_device);
                Soloud.Soloud_deinit(_device);
                _device = IntPtr.Zero;
            }
        }

        public override IntPtr CreateStream(string filename, bool looping)
        {
            IntPtr id = Soloud.WavStream_create();
            Soloud.WavStream_load(id, filename);
            Soloud.WavStream_setLooping(id, looping);
            if (id == IntPtr.Zero)
            {
                throw new Exception("Stream file not loaded: " + filename);
            }
            return id;
        }

        public override IntPtr LoadSound(string filename, bool looping)
        {
            IntPtr id = Soloud.Wav_create();
            Soloud.Wav_load(id, filename);
            Soloud.Wav_setLooping(id, looping);
            if (id == IntPtr.Zero)
            {
                throw new Exception("Sound file not loaded: " + filename);
            }
            return id;
        }

        public override void Step()
        {
            //empty
        }

        public override uint PlaySound(IntPtr id, uint channelId, bool paused)
        {
            if (id == IntPtr.Zero) return 0;
            return Soloud.Soloud_playEx(_device, id, 1.0f, 0.0f, paused, 0);
        }

		public override uint PlaySound(IntPtr id, uint channelId, bool paused, float volume, float pan)
		{
			if (id == IntPtr.Zero) return 0;
			return Soloud.Soloud_playEx(_device, id, volume, pan, paused, 0);
		}


        public override float GetChannelFrequency(uint channelId)
        {
            return Soloud.Soloud_getSamplerate(_device, channelId);
        }

        public override void SetChannelFrequency(uint channelId, float frequency)
        {
            Soloud.Soloud_setSamplerate(_device, channelId, frequency);
        }

        public override float GetChannelPan(uint channelId)
        {
            return Soloud.Soloud_getPan(_device, channelId);
        }

        public override void SetChannelPan(uint channelId, float pan)
        {
            Soloud.Soloud_setPan(_device, channelId, pan);
        }

        public override bool GetChannelPaused(uint channelId)
        {
            return Soloud.Soloud_getPause(_device, channelId);
        }

        public override void SetChannelPaused(uint channelId, bool pause)
        {
            Soloud.Soloud_setPause(_device, channelId, pause);
        }

        public override bool ChannelIsPlaying(uint channelId)
        {
            return (Soloud.Soloud_isValidVoiceHandle(_device, channelId) && !Soloud.Soloud_getPause(_device, channelId));
        }

        public override void StopChannel(uint channelId)
        {
            Soloud.Soloud_stop(_device, channelId);
        }

        public override float GetChannelVolume(uint channelId)
        {
            return Soloud.Soloud_getVolume(_device, channelId);
        }

        public override void SetChannelVolume(uint channelId, float volume)
        {
            Soloud.Soloud_setVolume(_device, channelId, volume);
        }

    }
}
