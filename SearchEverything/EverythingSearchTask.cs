using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SearchEverything.EverythingApi;

namespace SearchEverything
{
    public class EverythingSearchTask : VsSearchTask
    {
        private static readonly object SyncRoot = new object();
        private readonly SearchWindow _searchWindow;
        private SearchBoxInfo _searchBoxInfo;

        public EverythingSearchTask(uint dwCookie, IVsSearchQuery pSearchQuery,
            IVsSearchCallback pSearchCallback, SearchWindow searchWindow)
            : base(dwCookie, pSearchQuery, pSearchCallback)
        {
            _searchWindow = searchWindow;
            ThreadHelper.Generic.Invoke(() =>
            {
                var searchBox = (SearchBox)_searchWindow.Content;
                _searchBoxInfo = new SearchBoxInfo
                {
                    Width = searchBox.RenderSize.Width,
                    FontSize = searchBox.FontSize,
                    FontName = searchBox.FontFamily.ToString()
                };
            });
        }

        protected override void OnStartSearch()
        {
            // Get the search options.
            bool matchCase = _searchWindow.MatchCaseOption.Value;
            bool useRegex = _searchWindow.UseRegexOption.Value;
            bool includeFolders = _searchWindow.IncludeFolders.Value;

            // Set variables that are used in the finally block.
            uint resultCount = 0;
            ErrorCode = VSConstants.S_OK;
            List<SearchResult> contentItems = null;
            lock (SyncRoot)
            {
                try
                {
                    // Determine the results. 
                    var everythingApiManager = new EverythingApiManager
                    {
                        MatchCase = matchCase,
                        EnableRegex = useRegex,
                        IncludeFolders = includeFolders
                    };
                    contentItems = everythingApiManager.Search(SearchQuery.SearchString, _searchBoxInfo);
                    resultCount = (uint) contentItems.Count;

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
                        var resultListBox = ((SearchBox) _searchWindow.Content).ResultListBox;
                        resultListBox.ItemsSource = contentItems;
                    });

                    SearchResults = resultCount;
                }
            }
            // Call the implementation of this method in the base class. 
            // This sets the task status to complete and reports task completion. 
            base.OnStartSearch();
        }

        protected override void OnStopSearch()
        {
            SearchResults = 0;
            base.OnStopSearch();
        }
    }
}