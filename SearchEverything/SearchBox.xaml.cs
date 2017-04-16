using System.Windows.Controls;
using System.Windows.Input;

namespace SearchEverything
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox
    {
        public ListBox ResultListBox { get; }

        public SearchBox()
        {
            InitializeComponent();

            ResultListBox = RListBox;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                string fullPath = ((TextBlock) sender).Text;
                OpenFileManager.GetInstance().OpenDocumentInNewWindow(fullPath);
            }
        }
    }
}