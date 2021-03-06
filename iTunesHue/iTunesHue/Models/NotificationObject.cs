﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iTunesHue.Models
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetValue<T>(ref T field, T value, IEqualityComparer<T> comparer = null, [CallerMemberName] string propertyName = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            if (comparer.Equals(field, value)) return;
            field = value;
            RaisePropertyChanged(propertyName);
        }
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}