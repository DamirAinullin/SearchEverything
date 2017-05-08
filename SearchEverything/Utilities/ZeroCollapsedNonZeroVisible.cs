using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SearchEverything.Utilities
{
    public class ZeroCollapsedNonZeroVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rv = Visibility.Visible;
            int val;
            int.TryParse(value.ToString(), out val);
            if (val == 0)
            {
                rv = Visibility.Collapsed;
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}