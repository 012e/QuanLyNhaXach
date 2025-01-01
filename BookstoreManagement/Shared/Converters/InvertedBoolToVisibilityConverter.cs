using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookstoreManagement.Shared.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class InvertedBoolToVisibilityConverter : IValueConverter
{
    public Visibility TrueValue { get; } = Visibility.Visible;
    public Visibility FalseValue { get; } = Visibility.Collapsed;

    // set defaults

    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (!(value is bool))
            return null;
        return (bool)value ? FalseValue : TrueValue;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (Equals(value, FalseValue))
            return true;
        if (Equals(value, TrueValue))
            return false;
        return null;
    }
}
