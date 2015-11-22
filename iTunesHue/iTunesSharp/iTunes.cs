using System;
using System.Runtime.InteropServices;
using iTunesLib;

namespace iTunesSharp
{
    /// <summary>
    /// <see cref="iTunesApp"/> のマネージラッパーを提供します。
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class iTunes : IDisposable
    {
        private iTunesApp _app;
        //private Track _currentTrack;

        public int SoundVolume => _app.SoundVolume;

        //public event EventHandler<CurrentTrackChangedEventArgs> CurrentTrackChanged;
        //public event EventHandler PlayerPlay;
        //public event EventHandler PlayerStop;
        //public event EventHandler SoundVolumeChanged;

        public iTunes()
        {
            _app = new iTunesApp();
            //_currentTrack = new Track(_app.CurrentTrack);
            //_app.OnPlayerPlayEvent += app_OnPlayerPlayEvent;
            //_app.OnPlayerStopEvent += app_OnPlayerStopEvent;
            //_app.OnSoundVolumeChangedEvent += app_OnSoundVolumeChangedEvent;
        }

        public void Play() => _app.Play();
        public void Stop() => _app.Stop();
        public void NextTrack() => _app.NextTrack();
        public void PreviousTrack() => _app.PreviousTrack();
        public void Resume() => _app.Resume();
        public void Rewind() => _app.Rewind();
        public void Pause() => _app.Pause();

        public Track GetCurrentTrack()
        {
            ThrowIfDisposed();
            if (_app.CurrentTrack == null) return null;
            //return _currentTrack;
            return new Track(_app.CurrentTrack);
        }

        //private void app_OnSoundVolumeChangedEvent(int newVolume)
        //{
        //    OnSoundVolumChanged(new SoundVolumeChangedEventArgs(newVolume));
        //}
        //private void app_OnPlayerStopEvent(object iTrack)
        //{
        //    OnPlayerStop(EventArgs.Empty);
        //}
        //private void app_OnPlayerPlayEvent(object iTrack)
        //{
        //    OnPlayerPlay(EventArgs.Empty);

        //    var track = new Track((IITTrack)iTrack);

        //    if (_currentTrack == track) return;

        //    _currentTrack = track;
        //    OnCurrentTrackChanged(new CurrentTrackChangedEventArgs(_currentTrack));
        //}

        #region RaiseEvents
        //protected virtual void OnCurrentTrackChanged(CurrentTrackChangedEventArgs e)
        //{
        //    CurrentTrackChanged?.Invoke(this, e);
        //}
        //protected virtual void OnPlayerPlay(EventArgs e)
        //{
        //    PlayerPlay?.Invoke(this, e);
        //}
        //protected virtual void OnPlayerStop(EventArgs e)
        //{
        //    PlayerStop?.Invoke(this, e);
        //}
        //protected virtual void OnSoundVolumChanged(SoundVolumeChangedEventArgs e)
        //{
        //    SoundVolumeChanged?.Invoke(this, e);
        //}
        #endregion

        #region Dispose
        private bool _disposed;
        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(typeof(iTunes).FullName);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {

            }

            //_app.OnPlayerPlayEvent -= app_OnPlayerPlayEvent;
            //_app.OnPlayerStopEvent -= app_OnPlayerStopEvent;
            //_app.OnSoundVolumeChangedEvent -= app_OnSoundVolumeChangedEvent;
            Marshal.ReleaseComObject(_app);
            _app = null;
            _disposed = true;
        }
        ~iTunes()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}