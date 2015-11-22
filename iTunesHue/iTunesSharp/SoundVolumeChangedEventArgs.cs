using System;

namespace iTunesSharp
{
    public class SoundVolumeChangedEventArgs : EventArgs
    {
        public int NewVolume { get; }

        public SoundVolumeChangedEventArgs(int newVolume)
        {
            NewVolume = newVolume;
        }
    }
}