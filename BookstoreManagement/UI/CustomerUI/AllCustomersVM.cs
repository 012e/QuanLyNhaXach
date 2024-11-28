using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class AllCustomersVM : ListVM<Customer,EditCustomerVM>
    {
        protected override ApplicationDbContext Db { get; }
        protected override IContextualNavigatorService<EditCustomerVM, Customer> EditItemNavigator{ get; }
        protected INavigatorService<CreateCustomerVM> CreateCustomerNavigator { get; }
        [ObservableProperty]
        private string _searchText = "";
        public AllCustomersVM(ApplicationDbContext db,
            IContextualNavigatorService<EditCustomerVM,Customer> editCustomerNavigator,
            INavigatorService<CreateCustomerVM> createCustomerNavigator)
        {
            this.Db = db;
            this.EditItemNavigator = editCustomerNavigator;
            this.CreateCustomerNavigator = createCustomerNavigator;
        }
        protected override void LoadItems()
        {
            Db.ChangeTracker.Clear();
            var items = Db.Customers.OrderBy(customer => customer.Id).ToList();
            Items = new ObservableCollection<Customer>(items);
        }
        [RelayCommand]
        private void NavigateToCreateCustomer()
        {
            CreateCustomerNavigator.Navigate();
        }
       
        protected override bool FitlerItem(Customer item)
        {
            string name = item.FirstName + item.LastName;
            return name.ToLower().Contains(SearchText.ToLower());
        }
        partial void OnSearchTextChanged(string value)
        {
           ItemsView.Refresh();
        }
    }
}
