using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToastNotifications.Core;

namespace BookstoreManagement.UI.EmployeeUI
{
    public partial class AllEmployeeVM : ListVM<Employee, EditEmployeeVM>
    {
        protected override IContextualNavigatorService<EditEmployeeVM, Employee> EditItemNavigator { get; }
        protected INavigatorService<CreateEmployeeVM> createEmployeeNavigator;

        [ObservableProperty]
        private String _searchText = "";
        [ObservableProperty]
        private CurrentUserService _currentUserService;

        protected readonly ApplicationDbContext db;
        public AllEmployeeVM(ApplicationDbContext db,
            IContextualNavigatorService<EditEmployeeVM, Employee> editItemNavigator,
            INavigatorService<CreateEmployeeVM> createEmployeeNavigator , CurrentUserService currentUserService)
        {
            this.db = db;
            EditItemNavigator = editItemNavigator;
            this.createEmployeeNavigator = createEmployeeNavigator;
            this.CurrentUserService = currentUserService;
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
            SuccessNotification();
            throw new NotImplementedException();
        }

        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted employee successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
    }
}
