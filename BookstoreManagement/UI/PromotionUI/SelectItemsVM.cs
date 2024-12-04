using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.UI.PromotionUI
{
    public partial class SelectItemsVM : BaseViewModel
    {
        public SelectItemsVM()
        {
            
        }
        [RelayCommand]
        public void ConfirmSelection()
        {
            MessageBox.Show("Kka");
        }
        [RelayCommand]
        public void CloseModal()
        {
            MessageBox.Show("Hhehe");
        }
    }
}
