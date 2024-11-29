using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class EditCustomerVM : EditItemVM<Customer>
    {
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private Customer _item;
        private readonly INavigatorService<AllCustomersVM> customerNavigator;
        public EditCustomerVM(ApplicationDbContext db,
            INavigatorService<AllCustomersVM> customerNavigator)
        {
            this.db = db;
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
            Item = db.Customers.Find(id);
        }
        protected override void SubmitItemHandler()
        {
            db.Customers.Update(Item);
            db.SaveChanges();
        }
        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("submit success");
        }
    }

}
