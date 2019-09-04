using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1.Converters
{
    public class VisibilityConverter<T> : IValueConverter
    {
        public T True { get; set; }
        public T False { get; set; }

        public VisibilityConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as string != null)
            {
                return string.IsNullOrWhiteSpace(value as string) ? True : False;
            }
            if (value != null && value.GetType() == typeof(int))
            {
                return (int)value > 0 ? True : False;
            }
            return value == null ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return True;
        }
    }

    public sealed class NullToVisibilityConverter : VisibilityConverter<Visibility>
    {
        public NullToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
    }
}
