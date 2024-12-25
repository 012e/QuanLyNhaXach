using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.SettingUI
{
    public partial class AccountVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly CurrentUserService currentUserService;

        [ObservableProperty]
        private bool _isPasswordVisible = false;
        [ObservableProperty]
        private string _password;
        [ObservableProperty]
        private bool _isPasswordNotVisible = true;
        [ObservableProperty]
        private PackIconKind _passwordVisibilityIcon = PackIconKind.EyeOff;

        [RelayCommand]
        private void HideAndShow()
        {
            IsPasswordVisible = !IsPasswordVisible;
            IsPasswordNotVisible = !IsPasswordNotVisible;
            PasswordVisibilityIcon = IsPasswordVisible ? PackIconKind.Eye : PackIconKind.EyeOff;
        }
        public AccountVM(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
            ResetState();
        }
        [ObservableProperty]
        private string _currUserName;

        [ObservableProperty]
        private string _currUserPassword;

        public override void ResetState()
        {
            base.ResetState();
            LoadCurrentUser();
        }
        private void LoadCurrentUser()
        {
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = db.Employees
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == userId);

            if (userInfo != null)
            {
                CurrUserName = userInfo.Email;
                CurrUserPassword = userInfo.Password;
            }
            else
            {
                CurrUserName = string.Empty;
                CurrUserPassword = string.Empty;
            }
        }

        [ObservableProperty]
        private bool _isEnableEdit;

        [RelayCommand]
        private void Edit()
        {
            IsEnableEdit = true;
        }

        [RelayCommand]
        private async Task SaveEdit()
        {
            IsEnableEdit = false;
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = await db.Employees.FirstOrDefaultAsync(e => e.Id == userId);

            userInfo.Email = CurrUserName;
            userInfo.Password = CurrUserPassword;
            await db.SaveChangesAsync();
        }
    }
}
