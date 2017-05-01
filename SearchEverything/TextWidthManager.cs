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
                return "";
            }
            if (searchBoxInfo.Width <= 40 || text.Length <= 6)
            {
                return text;
            }
            int length = text.Length;
            double realWidth = searchBoxInfo.Width - 40;
            while (true) //0 length string will always fit
            {
                var testString = text.Substring(0, length);
                var formattedText = new FormattedText(testString,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(searchBoxInfo.FontName),
                    searchBoxInfo.FontSize,
                    Brushes.Black);
                
                if (formattedText.Width <= realWidth)
                {
                    break;
                }
                length--;
            }
            if (text.Length > length)
            {
                return text.Substring(0, 3) + "..." + text.Substring(text.Length - length + 6, length - 6);
            }
            return text;
        }
        
    }
}
