using BookstoreManagement.Core;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreManagement.DbContexts;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class CreateCustomerVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly INavigatorService<CustomerVM> customerNavigator;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _phoneNumber;

        [ObservableProperty]
        private string _email;

        [RelayCommand]

        // Them khach hang
        private void Submit()
        {
            var customer = new Customer
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            try
            {
                db.Add(customer);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                MessageBox.Show("Them khong thanh cong !!!");
                return;
            }
            MessageBox.Show("Them thanh cong");
        }

        // Dat gia tri mac dinh thong tin khach hang muon them
        private void ResetToDefaultValues()
        {
            FirstName = "NGUYEN VAN";
            LastName = "A";
            PhoneNumber = "0123456789";
            Email = "123456@gmail.com";
        }


        /// cap nhap trang thai
        public override void ResetState()
        {
            ResetToDefaultValues();
            base.ResetState();
        }

        // Nut tro ve lai
        [RelayCommand]
        private void GoBack()
        {
            customerNavigator.Navigate();
        }
        public CreateCustomerVM(ApplicationDbContext db, 
            INavigatorService<CustomerVM> customernNavigator)
        {
            ResetToDefaultValues();
            this.db = db;
            this.customerNavigator = customernNavigator;
        }
       
    }
}
