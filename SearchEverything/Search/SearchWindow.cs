using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SearchEverything.Search
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("7ac4b56d-6912-490d-8c41-28bb105edc8a")]
    public class SearchWindow : ToolWindowPane
    {
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public SearchWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            Caption = "Everything Explorer";
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            BitmapResourceID = 301;
            BitmapIndex = 3;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            Content = new Controls.SearchBox();
        }

        public override bool SearchEnabled => true;

        public override IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback)
        {
            if (pSearchQuery == null || pSearchCallback == null)
            {
                return null;
            }
            return new EverythingSearchTask(dwCookie, pSearchQuery, pSearchCallback, this);
        }

        public override void ClearSearch()
        {
            Controls.SearchBox control = (Controls.SearchBox)Content;
            control.ResultListBox.ItemsSource = null;
        }

        public override void ProvideSearchSettings(IVsUIDataSource pSearchSettings)
        {
            Microsoft.Internal.VisualStudio.PlatformUI.Utilities.SetValue(pSearchSettings, SearchSettingsDataSource.SearchStartTypeProperty.Name, (uint)VSSEARCHSTARTTYPE.SST_INSTANT);
            Microsoft.Internal.VisualStudio.PlatformUI.Utilities.SetValue(pSearchSettings, SearchSettingsDataSource.SearchProgressTypeProperty.Name, (uint)VSSEARCHPROGRESSTYPE.SPT_DETERMINATE);
        }

        public override IVsEnumWindowSearchFilters SearchFiltersEnum
        {
            get
            {
                var list = new List<IVsWindowSearchFilter>(EverythingWindowSearchFilter.Expressions.Count);
                foreach (var expression in EverythingWindowSearchFilter.Expressions)
                {
                    list.Add(new EverythingWindowSearchFilter(expression.Key, expression.Key));
                }
                return new WindowSearchFilterEnumerator(list);
            }
        }
    }
}

