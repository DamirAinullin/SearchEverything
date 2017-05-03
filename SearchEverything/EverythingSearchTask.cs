using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SearchEverything.EverythingApi;
using Tasks = System.Threading.Tasks;

namespace SearchEverything
{
    public class EverythingSearchTask : VsSearchTask
    {
        private static readonly object SyncRoot = new object();
        private static Tasks.Task _task;
        private static CancellationTokenSource _cancellationTokenSource;
        private readonly SearchWindow _searchWindow;
        private SearchBoxInfo _searchBoxInfo;

        public static CommandPackage CommandPackage { private get; set; }

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

            if (_task != null && (!_task.IsCompleted || _task.Status == Tasks.TaskStatus.Running ||
                _task.Status == Tasks.TaskStatus.WaitingToRun || _task.Status == Tasks.TaskStatus.WaitingForActivation))
            {
                _cancellationTokenSource.Cancel();
            }
            lock (SyncRoot)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _task = Tasks.Task.Factory.StartNew(() =>
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

                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        contentItems = everythingApiManager.Search(SearchQuery.SearchString,
                            _searchBoxInfo, _cancellationTokenSource.Token, CommandPackage.MaxNumberOfResults);

                        resultCount = (uint)contentItems.Count;
                        SearchCallback.ReportComplete(this, resultCount);
                    }
                    catch (Exception e)
                    {
                        ErrorCode = VSConstants.E_FAIL;
                    }
                    finally
                    {
                        ThreadHelper.Generic.Invoke(() =>
                        {
                            if (contentItems != null)
                            {
                                var resultListBox = ((SearchBox)_searchWindow.Content).ResultListBox;
                                resultListBox.ItemsSource = contentItems;
                            }
                        });

                        SearchResults = resultCount;
                    }
                }, _cancellationTokenSource.Token);
                try
                {
                    _task.Wait(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    //
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