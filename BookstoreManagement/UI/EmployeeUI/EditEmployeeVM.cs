    using System.Collections.ObjectModel;
using System.Security.RightsManagement;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.UI.EmployeeUI
{
    public partial class EditEmployeeVM : EditItemVM<Employee>
    {

        protected override ApplicationDbContext Db { get; }

        private readonly INavigatorService<AllEmployeeVM> allEmployeeNavigator;

        [ObservableProperty]
        private Employee _item;


        // Constructor
        public EditEmployeeVM(ApplicationDbContext db, INavigatorService<AllEmployeeVM> allEmployeeNavigator)
        {
            // Initialization code if needed
            this.Db = db;
            this.allEmployeeNavigator = allEmployeeNavigator;
        }

        [RelayCommand]
        private void NavigateBack()
        {
            allEmployeeNavigator.Navigate();
        }


        protected override void LoadItem()
        {
            Item = default;
            var id = ViewModelContext.Id;
            Item = Db.Employees.Find(id);
        }

        protected override void SubmitItemHandler()
        {
            Db.Employees.Update(Item);
            Db.SaveChanges();
        }

        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("submit success");
        }
    }
}
