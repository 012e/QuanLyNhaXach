using System.Globalization;
using System.Windows.Data;

namespace BookstoreManagement.Shared.Converters;

public class ScaleConverter : IValueConverter
{
    public double ScaleFactor { get; set; } = 0.3;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double originalSize)
        {
            return originalSize * ScaleFactor;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
