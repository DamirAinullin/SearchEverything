using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace SearchEverything
{
    public class TextWidthManager
    {
        public string GetSubStringForWidth(string text, SearchBoxInfo searchBoxInfo)
        {
            if (searchBoxInfo.Width <= 0)
            {
                return string.Empty;
            }
            if (searchBoxInfo.Width <= 55 || text.Length <= 6)
            {
                return text;
            }

            double realWidth = searchBoxInfo.Width - 55;
            var typeface = new Typeface(searchBoxInfo.FontName);

            int foundCharIndex = BinarySearch(
                text.Length,
                realWidth,
                (idxValue1, value2) =>
                {
                    var formattedText = new FormattedText(
                        //text.Substring(0, idxValue1 + 1),
                        text.Substring(0, 3) + "..." + text.Substring(text.Length - idxValue1 + 6, idxValue1 - 6),
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        searchBoxInfo.FontSize,
                        Brushes.Black,
                        null,
                        TextFormattingMode.Ideal);

                    return formattedText.WidthIncludingTrailingWhitespace.CompareTo(value2);
                });

            int numChars = foundCharIndex < 0 ? ~foundCharIndex : foundCharIndex + 1;

            if (text.Length > numChars)
            {
                return text.Substring(0, 3) + "..." + text.Substring(text.Length - numChars + 6, numChars - 6);
            }
            return text;
        }

        private int BinarySearch(
            int length,
            double value,
            Func<int, double, int> predicate)
        {
            int lo = 0;
            int hi = length - 1;

            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;

                int compareResult = predicate(mid, value);

                if (compareResult == 0)
                {
                    return mid;
                }
                if (compareResult < 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return ~lo;
        }

    }
}
