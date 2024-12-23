using System.Windows;
using System.Windows.Controls;

namespace BookstoreManagement.LoginUI
{
    public partial class LoginV : UserControl
    {
        public LoginV()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = (LoginVM)this.DataContext; 
            viewModel.Password = passwordBox.Password; 
        }
    }
}
