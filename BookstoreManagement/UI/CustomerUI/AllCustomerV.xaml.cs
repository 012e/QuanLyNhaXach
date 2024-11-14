using System.Windows.Controls;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class AllCustomersV : UserControl
    {
        public AllCustomersV()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtSearch.Focus();
        }
    }
}
