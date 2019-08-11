using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
//using Windows.UI.Xaml.Data;

namespace WpfApp1.Converters
{
    /// <summary>
    /// Used in ChatLineControl.xaml to reduce border width.
    /// </summary>
    class ChatLineWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 0.85;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 0.85;
        }
    }
}
