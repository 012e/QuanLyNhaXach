using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.SettingUI
{
    public partial class MyProfileVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly CurrentUserService currentUserService;
        public MyProfileVM(ApplicationDbContext db,
            CurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
        }
    }
}
