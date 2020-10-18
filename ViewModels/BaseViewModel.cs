using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageTagger.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void AnnounceIt(Action setAction, [CallerMemberName] string propertyName = "")
        {
            setAction();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}