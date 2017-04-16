using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SearchEverything.EverythingApi;

namespace SearchEverything
{
    internal class EverythingSearchTask : VsSearchTask
    {
        private readonly SearchWindow _searchWindow;

        public EverythingSearchTask(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback, SearchWindow searchWindow)
            : base(dwCookie, pSearchQuery, pSearchCallback)
        {
            _searchWindow = searchWindow;
        }

        protected override void OnStartSearch()
        {
            // Use the original content of the text box as the target of the search. 
            //var separator = new[] { Environment.NewLine };
            //string[] contentArr = ((SearchBox)_searchWindow.Content).SearchContent.Split(separator, StringSplitOptions.None);

            // Get the search option. 
            var matchCase = _searchWindow.MatchCaseOption.Value;

            // Set variables that are used in the finally block.
            StringBuilder searchContentBuilder = new StringBuilder("");
            uint resultCount = 0;
            ErrorCode = VSConstants.S_OK;

            try
            {
                string searchString = SearchQuery.SearchString;

                // If the search string contains the filter string, filter the content array. 
               /* string filterString = "lines:\"even\"";

                if (SearchQuery.SearchString.Contains(filterString))
                {
                    // Retain only the even items in the array.
                    contentArr = GetEvenItems(contentArr);

                    // Remove 'lines:"even"' from the search string.
                    searchString = RemoveFromString(searchString, filterString);
                }*/
                lock (_searchWindow)
                {
                    // Determine the results. 
                    var everythingApiManager = new EverythingApiManager();

                    var contentItems = everythingApiManager.Search(searchString);
                    foreach (var item in contentItems)
                    {
                        searchContentBuilder.Append(item.FullPath);
                        searchContentBuilder.Append("\n");
                    }
                }
                /*uint progress = 0;
                foreach (string line in contentArr)
                {
                    if (matchCase)
                    {
                        if (line.Contains(searchString))
                        {
                            sb.AppendLine(line);
                            resultCount++;
                        }
                    }
                    else
                    {
                        if (line.ToLower().Contains(searchString.ToLower()))
                        {
                            sb.AppendLine(line);
                            resultCount++;
                        }
                    }
                    

                    SearchCallback.ReportProgress(this, progress++, (uint)contentArr.GetLength(0));

                    // Uncomment the following line to demonstrate the progress bar. 
                    // System.Threading.Thread.Sleep(100);
                }*/
            }
            catch (Exception e)
            {
                ErrorCode = VSConstants.E_FAIL;
            }
            finally
            {
                ThreadHelper.Generic.Invoke(() =>
                    { ((SearchBox)_searchWindow.Content).SearchResultsTextBox.Text = searchContentBuilder.ToString(); });

                SearchResults = resultCount;
            }

            // Call the implementation of this method in the base class. 
            // This sets the task status to complete and reports task completion. 
            base.OnStartSearch();
        }

        protected override void OnStopSearch()
        {
            SearchResults = 0;
        }

        private string RemoveFromString(string origString, string stringToRemove)
        {
            int index = origString.IndexOf(stringToRemove);
            if (index == -1)
            {
                return origString;
            }
            return origString.Substring(0, index) + origString.Substring(index + stringToRemove.Length);
        }

        private string[] GetEvenItems(string[] contentArr)
        {
            int length = contentArr.Length / 2;
            string[] evenContentArr = new string[length];

            int indexB = 0;
            for (int index = 1; index < contentArr.Length; index += 2)
            {
                evenContentArr[indexB] = contentArr[index];
                indexB++;
            }

            return evenContentArr;
        }
    }
}