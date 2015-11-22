using System.IO;
using System.Threading.Tasks;
using iTunesLib;

namespace iTunesSharp
{
    public class Artwork
    {
        // ReSharper disable once InconsistentNaming
        internal readonly IITArtwork _artwork;

        public string Description => _artwork.Description;
        public bool IsDownloadedArtwork => _artwork.IsDownloadedArtwork;

        public Artwork(IITArtwork artwork)
        {
            _artwork = artwork;
        }

        public async Task<Stream> GetStreamAsync()
        {
            var file = Path.GetTempFileName();
            try
            {
                _artwork.SaveArtworkToFile(file);

                using (var stream = File.OpenRead(file))
                {
                    var memory = new MemoryStream();
                    await stream.CopyToAsync(memory).ConfigureAwait(false);
                    memory.Seek(0, SeekOrigin.Begin);
                    return memory;
                }
            }
            finally
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
