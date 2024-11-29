using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookstoreManagement.UI.EmployeeUI
{
    public partial class AllEmployeeVM : ListVM<Employee, EditEmployeeVM>
    {
        protected override IContextualNavigatorService<EditEmployeeVM, Employee> EditItemNavigator { get; }
        protected INavigatorService<CreateEmployeeVM> createEmployeeNavigator;

        [ObservableProperty]
        private String _searchText = "";

        protected readonly ApplicationDbContext db;
        public AllEmployeeVM(ApplicationDbContext db,
            IContextualNavigatorService<EditEmployeeVM, Employee> editItemNavigator,
            INavigatorService<CreateEmployeeVM> createEmployeeNavigator)
        {
            this.db = db;
            EditItemNavigator = editItemNavigator;
            this.createEmployeeNavigator = createEmployeeNavigator;
        }
        [RelayCommand]
        protected void NavigateToCreateEmployee()
        {
            createEmployeeNavigator.Navigate();
        }
        protected override void LoadItems()
        {
            db.ChangeTracker.Clear();
            var items = db.Employees.OrderBy(employee => employee.Id).ToList();
            Items = new ObservableCollection<Employee>(items);
        }
        protected override bool FitlerItem(Employee item)
        {
            string name = item.FirstName + item.LastName;
            return name.ToLower().Contains(SearchText.ToLower());
        }

        partial void OnSearchTextChanged(string value)
        {
            ItemsView.Refresh();
        }

        protected override void DeleteItem(Employee item)
        {
            throw new NotImplementedException();
        }
    }
}
