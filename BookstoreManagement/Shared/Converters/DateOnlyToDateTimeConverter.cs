using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookstoreManagement.Shared.Converters
{
    [ValueConversion(typeof(DateOnly), typeof(DateTime))]
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                // Chuyển DateOnly thành DateTime
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            return DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Chuyển DateTime thành DateOnly
                return DateOnly.FromDateTime(dateTime);
            }
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
