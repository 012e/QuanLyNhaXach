using System.Windows.Controls;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class CustomerV : UserControl
    {
        public CustomerV()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtSearch.Focus();
        }
    }
}
