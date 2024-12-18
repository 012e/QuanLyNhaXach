using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookstoreManagement.Shared.Converters;

[ValueConversion(typeof(decimal), typeof(string))]
public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Binding.DoNothing;

        try
        {
            decimal amountInVND = System.Convert.ToDecimal(value);
            return amountInVND.ToString("N0", culture) + "đ"; // Format with thousands separator
        }
        catch (Exception)
        {
            return Binding.DoNothing;
        }
    }

    // Convert back is not supported in this case
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
