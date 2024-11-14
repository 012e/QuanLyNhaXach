using BookstoreManagement.Core;
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
    public partial class EditCustomerVM : BaseViewModel, IContextualViewModel<int>
    {
        public int ViewModelContext { get; set; }

        private readonly ApplicationDbContext db; // co so du lieu

        [ObservableProperty]
        private ObservableCollection<AllCustomersVM> _customers;   // danh sach khach hang

        [ObservableProperty]
        private Customer _customer; // customer can chinh sua

        // posepond: command GoBack
        private readonly INavigatorService<AllCustomersVM> customerNavigator;

        public EditCustomerVM(ApplicationDbContext db,
            INavigatorService<AllCustomersVM> customerNavigator)
        {
            this.db = db;
            this.customerNavigator = customerNavigator;
            
        }
        public override void ResetState()
        {
            base.ResetState();
            LoadCustomer();


        }

        // save infor
        [RelayCommand]
        private void Submit()
        {
            if (Customer != null)
            {
                db.Update(Customer);
                db.SaveChanges();
                LoadCustomer();
                MessageBox.Show("Submit sussesful!");
            }
        }

        // back command
        [RelayCommand]
        private void GoBack()
        {
            customerNavigator.Navigate();
        }

        // load customer
        private void LoadCustomer()
        {
            var id = ViewModelContext;
            Customer = db.Customers.Find(id);

            if (Customer == null) return;

        }

    }

}
