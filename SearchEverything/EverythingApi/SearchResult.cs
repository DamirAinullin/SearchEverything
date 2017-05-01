using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SearchEverything.EverythingApi
{
    public class SearchResult :INotifyPropertyChanged
    {
        private string _showPath;
        public string FullPath { get; set; }

        public string ShowPath
        {
            get { return _showPath; }
            set { _showPath = value; OnPropertyChanged(); }
        }

        public ResultType Type { get; set; }
        public ImageSource ImageSource { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}