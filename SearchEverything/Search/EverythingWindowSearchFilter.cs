using System.Collections.Generic;
using Microsoft.VisualStudio.PlatformUI;

namespace SearchEverything.Search
{
    public class EverythingWindowSearchFilter : WindowSearchCustomFilter
    {
        private readonly string _displayText;

        public static Dictionary<string, string> Expressions { get; } = new Dictionary<string, string>
        {
            {"Match case", "case:"}, {"Ignore case", "nocase:"},
            {"Match files only", "file:"}, {"Match folders only", "folder:"},
            {"Enable regex.", "regex:"}, {"Disable regex.", "noregex:"},
            {"Match the full path", ":path"}, {"Match just the filename", ":nopath"},
            {"Enable wildcards", "wildcards:"}, {"Disable wildcards", "nowildcards:"},
            {"Match accent marks", "diacritics:"}, {"Ignore accent marks", "nodiacritics:"},
            {"Match the whole filename", "wfn:"}, {"Match anywhere in the filename", "nowfn:"},
            {"Enable fast ASCII case comparison", "ascii:"}, {"Disable fast ASCII case comparison", "noascii:"}
        };

        public EverythingWindowSearchFilter(string displayText, string tooltip) : base(displayText, tooltip)
        {
            _displayText = displayText;
        }

        public override void ApplyFilter(ref string searchString, ref int selectionStart, ref int selectionEnd)
        {
            searchString = AddToSearchString(searchString, Expressions[_displayText]);
            base.ApplyFilter(ref searchString, ref selectionStart, ref selectionEnd);
        }

        private string AddToSearchString(string searchString, string filter)
        {
            if (searchString.IndexOf(':') != -1)
            {
                return filter + " " + searchString;
            }
            return filter + searchString;
        }
    }
}
