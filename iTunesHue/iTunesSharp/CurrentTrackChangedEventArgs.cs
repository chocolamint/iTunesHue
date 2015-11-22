using System;

namespace iTunesSharp
{
    public class CurrentTrackChangedEventArgs : EventArgs
    {
        public Track Track { get; }

        public CurrentTrackChangedEventArgs(Track track)
        {
            Track = track;
        }
    }
}