using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class EditCustomerVM : EditItemVM<Customer>
    {
        protected override ApplicationDbContext Db { get; }
        [ObservableProperty]
        private Customer _item;
        private readonly INavigatorService<AllCustomersVM> customerNavigator;
        public EditCustomerVM(ApplicationDbContext db,
            INavigatorService<AllCustomersVM> customerNavigator)
        {
            this.Db = db;
            this.customerNavigator = customerNavigator;
        }
        [RelayCommand]
        private void GoBack()
        {
            customerNavigator.Navigate();
        }

        // load customer
        protected override void LoadItem()
        {
            Item = default;
            var id = ViewModelContext.Id;
            Item = Db.Customers.Find(id);
        }
        protected override void SubmitItemHandler()
        {
            Db.Customers.Update(Item);
            Db.SaveChanges();
        }
        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("submit success");
        }
    }

}
