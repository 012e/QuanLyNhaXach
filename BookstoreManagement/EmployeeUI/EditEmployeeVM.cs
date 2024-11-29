using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.EmployeeUI
{
    public partial class EditEmployeeVM : EditItemVM<Employee>
    {

        private readonly ApplicationDbContext db;

        private readonly INavigatorService<AllEmployeeVM> allEmployeeNavigator;

        [ObservableProperty]
        private Employee _item;


        // Constructor
        public EditEmployeeVM(ApplicationDbContext db, INavigatorService<AllEmployeeVM> allEmployeeNavigator)
        {
            // Initialization code if needed
            this.db = db;
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
            Item = db.Employees.Find(id);
        }

        protected override void SubmitItemHandler()
        {
            db.Employees.Update(Item);
            db.SaveChanges();
        }

        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("submit success");
        }
    }
}
