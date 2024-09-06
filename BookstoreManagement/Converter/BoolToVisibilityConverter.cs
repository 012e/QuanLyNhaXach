using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BookstoreManagement.Converter;

[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public Visibility TrueValue { get; } = Visibility.Visible;
    public Visibility FalseValue { get; } = Visibility.Collapsed;

    // set defaults

    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (!(value is bool))
            return null;
        return (bool)value ? TrueValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (Equals(value, TrueValue))
            return true;
        if (Equals(value, FalseValue))
            return false;
        return null;
    }
}
