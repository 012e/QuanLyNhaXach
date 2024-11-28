using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
    public partial class AllEmployeeVM : ListVM<Employee, EditEmployeeVM>
    {
        protected override IContextualNavigatorService<EditEmployeeVM, Employee> EditItemNavigator { get; }
        protected INavigatorService<CreateEmployeeVM> createEmployeeNavigator;

        [ObservableProperty]
        private String _searchText = "";

        protected override ApplicationDbContext Db { get; }
        public AllEmployeeVM(ApplicationDbContext db, 
            IContextualNavigatorService<EditEmployeeVM, Employee> editItemNavigator,
            INavigatorService<CreateEmployeeVM> createEmployeeNavigator)
        {
            Db = db;
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
            Db.ChangeTracker.Clear();
            var items = Db.Employees.OrderBy(employee => employee.Id).ToList();
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
    }
}
