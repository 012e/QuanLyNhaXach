using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BookstoreManagement.Shared.Models;

namespace BookstoreManagement.Shared.Converters;

[ValueConversion(typeof(ICollection<Tag>), typeof(string))]
public class ListToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ICollection<Tag> tagCollection)
        {
            // Chuyển danh sách Tags thành chuỗi, sử dụng dấu phân cách là dấu phẩy
            return string.Join(", ", tagCollection.Select(t => t.Name));
        }
        return string.Empty;  // Nếu không phải ICollection<Tag>, trả về chuỗi rỗng
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Nếu bạn cần chuyển chuỗi lại thành danh sách Tag, bạn có thể triển khai tại đây
        return null;
    }
}