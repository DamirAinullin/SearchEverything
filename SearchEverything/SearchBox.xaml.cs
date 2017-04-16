using System.Windows.Controls;

namespace SearchEverything
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox
    {
        public TextBox SearchResultsTextBox { get; }

        public string SearchContent
        {
            get { return SearchResultsTextBox.Text; }
            set { SearchResultsTextBox.Text = value; }
        }

        public SearchBox()
        {
            InitializeComponent();

            SearchResultsTextBox = resultsTextBox;
            /*SearchContent = BuildContent();

            SearchResultsTextBox.Text = SearchContent;*/
        }
        /*
        private string BuildContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("1 go");
            sb.AppendLine("2 good");
            sb.AppendLine("3 Go");
            sb.AppendLine("4 Good");
            sb.AppendLine("5 goodbye");
            sb.AppendLine("6 Goodbye");
            sb.AppendLine("7 goo");
            sb.AppendLine("8 Goo");
            sb.AppendLine("9 God");

            return sb.ToString();
        }*/
    }
}