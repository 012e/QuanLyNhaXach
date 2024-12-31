using System.Globalization;
using System.Windows.Data;
namespace BookstoreManagement.Shared.Converters;

[ValueConversion(typeof(float), typeof(string))]
public sealed class FloatToPercentageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float || value is double || value is decimal)
        {
            return $"{System.Convert.ToDecimal(value) * 100:0.#}%";
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string stringValue)
        {
            return null;
        }

        stringValue = stringValue.Trim().TrimEnd('%');
        if (float.TryParse(stringValue.TrimEnd('%'), NumberStyles.Any, culture, out float result))
        {
            return result / 100f;
        }

        return null;
    }
}
