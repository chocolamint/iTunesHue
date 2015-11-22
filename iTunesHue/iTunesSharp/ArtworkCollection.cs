using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iTunesLib;

namespace iTunesSharp
{
    public class ArtworkCollection : IReadOnlyList<Artwork>, ICollection<Artwork>
    {
        private readonly IITArtwork[] _artworkCollection;
        private readonly Artwork[] _wrapper;
        public int Count => _artworkCollection.Length;

        bool ICollection<Artwork>.IsReadOnly => true;

        public Artwork this[int index] =>
            _wrapper[index] ?? (_wrapper[index] = new Artwork(_artworkCollection[index]));

        public ArtworkCollection(IITArtworkCollection artworkCollection)
        {
            _artworkCollection = artworkCollection.Cast<IITArtwork>().ToArray();
            _wrapper = new Artwork[_artworkCollection.Length];
        }

        public IEnumerator<Artwork> GetEnumerator()
        {
            var count = _artworkCollection.Length;

            for (var i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        #region IEnumerable members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region ICollection<T> members
        void ICollection<Artwork>.Add(Artwork item)
        {
            throw new NotSupportedException();
        }

        void ICollection<Artwork>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<Artwork>.Contains(Artwork item)
        {
            return this.Any(x => x._artwork == item._artwork);
        }

        void ICollection<Artwork>.CopyTo(Artwork[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        bool ICollection<Artwork>.Remove(Artwork item)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}