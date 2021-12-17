using System;

namespace GXPEngine.Core
{
    public class FMODSoundSystem : SoundSystem
    {
        private static IntPtr _system = IntPtr.Zero;

        private IntPtr GetSystem()
        {
            if (_system == IntPtr.Zero)
            {
                // if fmod not initialized, create system and init default
                FMOD.System_Create(out _system);
                FMOD.System_Init(_system, 32, 0, 0);
            }
            return _system;
        }

        public override void Init()
        {
            //setup is done on-demand
        }

        public override void Deinit()
        {
            //clean up is never done?
        }

        public override IntPtr CreateStream(string filename, bool looping)
        {
            uint loop = FMOD.FMOD_LOOP_OFF; // no loop
            if (looping) loop = FMOD.FMOD_LOOP_NORMAL;

			IntPtr id;
            FMOD.System_CreateStream(GetSystem(), filename, loop, 0, out id);
            if (id == IntPtr.Zero)
            {
                throw new Exception("Sound file not found: " + filename);
            }
            return id;
        }

        public override IntPtr LoadSound(string filename, bool looping)
        {
            uint loop = FMOD.FMOD_LOOP_OFF; // no loop
            if (looping) loop = FMOD.FMOD_LOOP_NORMAL;

			IntPtr id;
            FMOD.System_CreateSound(GetSystem(), filename, loop, 0, out id);
            return id;
        }

        public override void Step()
        {
            if (_system != IntPtr.Zero)
            {
                FMOD.System_Update(_system);
            }
        }

        public override uint PlaySound(IntPtr id, uint channelId, bool paused)
        {
			uint outId;
            FMOD.System_PlaySound(GetSystem(), channelId, id, paused, out outId);
            return outId;
        }

		public override uint PlaySound(IntPtr id, uint channelId, bool paused, float volume, float pan)
		{
			uint outId;
			FMOD.System_PlaySound(GetSystem(), channelId, id, true, out outId);
			SetChannelVolume (outId, volume);
			SetChannelPan (outId, pan);
			SetChannelPaused (outId, paused);
			return outId;
		}

        public override float GetChannelFrequency(uint channelId)
        {
			float frequency;
            FMOD.Channel_GetFrequency(channelId, out frequency);
            return frequency;
        }

        public override void SetChannelFrequency(uint channelId, float frequency)
        {
            FMOD.Channel_SetFrequency(channelId, frequency);
        }

        public override float GetChannelPan(uint channelId)
        {
			float pan;
            FMOD.Channel_GetPan(channelId, out pan);
            return pan;
        }

        public override void SetChannelPan(uint channelId, float pan)
        {
            FMOD.Channel_SetPan(channelId, pan);
        }

        public override bool GetChannelPaused(uint channelId)
        {
			bool pause;
            FMOD.Channel_GetPaused(channelId, out pause);
            return pause;
        }

        public override void SetChannelPaused(uint channelId, bool pause)
        {
            FMOD.Channel_SetPaused(channelId, pause);
        }

        public override bool ChannelIsPlaying(uint channelId)
        {
			bool playing;
            FMOD.Channel_IsPlaying(channelId, out playing);
            return playing;
        }

        public override void StopChannel(uint channelId)
        {
            FMOD.Channel_Stop(channelId);
        }

        public override float GetChannelVolume(uint channelId)
        {
			float volume;
            FMOD.Channel_GetVolume(channelId, out volume);
            return volume;
        }

        public override void SetChannelVolume(uint channelId, float volume)
        {
            FMOD.Channel_SetVolume(channelId, volume);
        }

    }
}
