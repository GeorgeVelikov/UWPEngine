using System.Collections.Specialized;
using System.ComponentModel;

namespace UWPEngine.Utility {
    public class NotifyPropertyChangedBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
