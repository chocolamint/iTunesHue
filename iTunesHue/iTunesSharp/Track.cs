using System;
using iTunesLib;

namespace iTunesSharp
{
    /// <summary>
    /// トラックを表します。
    /// </summary>
    public class Track : IEquatable<Track>
    {
        private readonly IITTrack _track;

        public string Name => _track.Name;
        public string Album => _track.Album;
        public ArtworkCollection Artwork { get; }

        public Track(IITTrack track)
        {
            if (track == null) throw new ArgumentNullException(nameof(track));

            _track = track;
            Artwork = new ArtworkCollection(track.Artwork);
        }

        public bool Equals(Track other) => _track.trackID == other?._track.trackID;
        public override bool Equals(object obj) => Equals(obj as Track);
        public override int GetHashCode() => _track.trackID;
        public static bool operator ==(Track left, Track right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, right) || left.Equals(right);
        }
        public static bool operator !=(Track left, Track right) => !(left == right);
    }
}