using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BookstoreManagement.Shared.Models;
using Sprache;

namespace BookstoreManagement.Shared.Converters;
[ValueConversion(typeof(IEnumerable), typeof(string))]
public class ListToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable list)
        {
            // Kiểm tra xem list có phần tử không
            if (list.Cast<object>().Any())
            {
                Debug.WriteLine("List has items.");
            }
            else
            {
                Debug.WriteLine("List is empty.");
            }

            var propertyName = parameter?.ToString();
            var items = list.Cast<object>()
                            .Select(item => string.IsNullOrEmpty(propertyName)
                                ? item.ToString()
                                : item?.GetType().GetProperty(propertyName)?.GetValue(item, null)?.ToString());

            var result = string.Join(", ", items);
            Debug.WriteLine($"Converted value: {result}");  // In ra giá trị chuyển đổi
            return result;
        }
        return string.Empty;  // Trả về chuỗi rỗng nếu không phải danh sách
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue)
        {
            // Loại bỏ dấu phẩy thừa (nếu có) ở đầu và cuối chuỗi, và tách chuỗi thành danh sách các mục
            strValue = strValue.Trim().TrimEnd(',');

            // Tách chuỗi thành danh sách các mục, dựa trên dấu phẩy
            var items = strValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(item => item.Trim())  // Loại bỏ khoảng trắng thừa quanh các mục
                                .Select(name => new Tag { Name = name }) // Chuyển mỗi phần tử thành đối tượng Tag
                                .ToList();

            // Trả về ObservableCollection<Tag>
            return new ObservableCollection<Tag>(items);
        }

        return null; // Trả về null nếu không thể chuyển đổi
    }
}
