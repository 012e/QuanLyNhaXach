using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToastNotifications.Core;
using ToastNotifications.Messages.Error;

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
            WarningNotification();
            IsEnableEdit = true;
        }
        
        [RelayCommand]
        private async Task SaveEdit()
        {
            if (!Check_Valid_Input())
            {
                ErrorNotification();
                return;
            }
            IsEnableEdit = false;
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = await db.Employees.FirstOrDefaultAsync(e => e.Id == userId);

            userInfo.Email = CurrUserName;
            userInfo.Password = CurrUserPassword;
            SuccessNotification();
            await db.SaveChangesAsync();
        }

        [ObservableProperty]
        private string _errorMessage = string.Empty;
        private bool IsValidEmail(string input)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(input, emailPattern);
        }

        private bool Check_Valid_Input()
        {
            if (string.IsNullOrWhiteSpace(CurrUserName))
            {
                ErrorMessage = "User name can not be empty!";
                return false;
            }
            if (!IsValidEmail(CurrUserName))
            {
                ErrorMessage = "User name is not a valid type(example@example.com)!";
                return false;
            }
            if (!Is_Password_Valid())
            {
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
        private bool Is_Password_Valid()
        {
            string password = CurrUserPassword;
            if (password.Length < 8)
            {
                ErrorMessage = "Password must be at least 8 characters long!";
                return false;
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                ErrorMessage = "Password must contain at least one uppercase letter!";
                return false;
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                ErrorMessage = "Password must contain at least one lowercase letter!";
                return false;
            }
            if (!Regex.IsMatch(password, @"\d"))
            {
                ErrorMessage = "Password must contain at least one digit!";
                return false;
            }
            if (!Regex.IsMatch(password, @"[!@#$%^&*()\-_=+\[\]{};:'"",.<>?/\\]"))
            {
                ErrorMessage = "Password must contain at least special character!";
                return false;
            }
            if (password.Contains(" "))
            {
                ErrorMessage = "Password must not contain any spaces!";
                return false;
            }
            return true;
        }
        private void WarningNotification()
        {
            GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Save Successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void ErrorNotification()
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", ErrorMessage, NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
    }
}
