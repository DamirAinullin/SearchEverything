using System;
using System.Collections.Generic;
using System.Linq;
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
            // Get the search options.
            bool matchCase = _searchWindow.MatchCaseOption.Value;
            bool useRegex = _searchWindow.UseRegexOption.Value;

            // Set variables that are used in the finally block.
            uint resultCount = 0;
            ErrorCode = VSConstants.S_OK;
            List<SearchResult> contentItems = new List<SearchResult>();
            try
            {
                lock (_searchWindow)
                {
                    // Determine the results. 
                    var everythingApiManager = new EverythingApiManager
                    {
                        MatchCase = matchCase,
                        EnableRegex = useRegex
                    };
                    contentItems = everythingApiManager.Search(SearchQuery.SearchString).ToList();
                    resultCount = (uint)contentItems.Count;
                }
                //SearchCallback.ReportProgress(this, progress++, (uint)contentArr.GetLength(0));
            }
            catch (Exception e)
            {
                ErrorCode = VSConstants.E_FAIL;
            }
            finally
            {
                ThreadHelper.Generic.Invoke(() =>
                {
                    ((SearchBox) _searchWindow.Content).ResultListBox.ItemsSource = contentItems;
                });

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
    }
}