using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookstoreManagement.SettingUI
{
    /// <summary>
    /// Interaction logic for AccountV.xaml
    /// </summary>
    public partial class AccountV : UserControl
    {
        public AccountV()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //var viewModel = (AccountVM)this.DataContext;
            //viewModel.Password = passwordBox.Password;
            if (this.DataContext is AccountVM viewModel)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
