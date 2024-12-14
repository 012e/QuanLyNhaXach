using BookstoreManagement.LoginUI.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BookstoreManagement.LoginUI.Converters
{


    [ValueConversion(typeof(object[]), typeof(LoginDto))]
    public class LoginDtoConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null;

            string email = (values[0] as TextBox)?.Text;
            string password = (values[1] as PasswordBox)?.Password;

            if (email == null || password == null)
                return null;

            return new LoginDto
            {
                Email = email,
                Password = password
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
