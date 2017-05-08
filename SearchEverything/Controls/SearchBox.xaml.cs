using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SearchEverything.EverythingApi;
using SearchEverything.Search;
using SearchEverything.Utilities;

namespace SearchEverything.Controls
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
                string fullPath = ((Grid)((TextBlock)sender).Parent).ToolTip.ToString();
                var openFileManager = OpenFileManager.GetInstance();
                if (Directory.Exists(fullPath))
                {
                    openFileManager.OpenFileInExplorer(fullPath);
                }
                else
                {
                    openFileManager.OpenFileInVisualStudio(fullPath);
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void OpenInVisualStudio_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                return;
            }
            ContextMenu parentContextMenu = menuItem.CommandParameter as ContextMenu;
            var grid = parentContextMenu?.PlacementTarget as Grid;
            if (grid == null)
            {
                return;
            }
            string fullPath = grid.ToolTip.ToString();
            OpenFileManager.GetInstance().OpenFileInVisualStudio(fullPath);
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                return;
            }
            ContextMenu parentContextMenu = menuItem.CommandParameter as ContextMenu;
            var grid = parentContextMenu?.PlacementTarget as Grid;
            if (grid == null)
            {
                return;
            }
            string fullPath = grid.ToolTip.ToString();
            OpenFileManager.GetInstance().OpenFileInExplorer(fullPath);
        }

        private void OpenInDefaultProgram_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                return;
            }
            ContextMenu parentContextMenu = menuItem.CommandParameter as ContextMenu;
            var grid = parentContextMenu?.PlacementTarget as Grid;
            if (grid == null)
            {
                return;
            }
            string fullPath = grid.ToolTip.ToString();
            OpenFileManager.GetInstance().OpenInDefaultProgram(fullPath);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
            {
                return;
            }
            if (ResultListBox.ItemsSource == null)
            {
                return;
            }
            var textWidthManager = new TextWidthManager();
            foreach (SearchResult item in ResultListBox.ItemsSource)
            {
                var searchBoxInfo = new SearchBoxInfo
                {
                    Width = e.NewSize.Width,
                    FontSize = ResultListBox.FontSize,
                    FontName = ResultListBox.FontFamily.ToString()
                };
                item.ShowPath = textWidthManager.GetSubStringForWidth(item.FullPath, searchBoxInfo);
            }
        }
    }
}