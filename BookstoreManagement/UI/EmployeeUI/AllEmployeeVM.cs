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

        protected override ApplicationDbContext Db { get; }
        public AllEmployeeVM(ApplicationDbContext db, 
            IContextualNavigatorService<EditEmployeeVM, Employee> editItemNavigator)
        {
            Db = db;
            EditItemNavigator = editItemNavigator;
        }
    }
}
