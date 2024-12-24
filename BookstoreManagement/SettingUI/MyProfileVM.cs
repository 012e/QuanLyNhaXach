using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            ResetState();
        }

        [ObservableProperty]
        private string _userFirstName;

        [ObservableProperty]
        private string _userLastName;

        [ObservableProperty]
        private string _userEmail;

        [ObservableProperty]
        private bool _userRoll;

        [ObservableProperty]
        private string _userRollText;

        [ObservableProperty]
        private bool _userGender;

        [ObservableProperty]
        private DateOnly _userBirthDay;

        [ObservableProperty]
        private string _userAddress;

        [ObservableProperty]
        private string _userPhone;
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
                UserFirstName = userInfo.FirstName;

                UserLastName = userInfo.LastName;

                UserEmail = userInfo.Email;

                UserRollText = userInfo.IsManager ? "Manager" : "Employee";

                UserBirthDay = userInfo.Birthday;

                UserGender = userInfo.Gender;

                UserAddress = userInfo.Address;

                UserPhone = userInfo.PhoneNumber;

                LoadImageFromUrl(userInfo.ProfilePicture);
            }
            else
            {
                UserFirstName = string.Empty;
                UserLastName = string.Empty;
                UserEmail = string.Empty;
                UserRollText = "Unknown";
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
        private void SaveEdit()
        {
            IsEnableEdit = false;
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = db.Employees.FirstOrDefault(e => e.Id == userId);

            userInfo.FirstName = UserFirstName;
            userInfo.LastName = UserLastName;
            userInfo.PhoneNumber = UserPhone;
            userInfo.Email = UserEmail;
            userInfo.Birthday = UserBirthDay;
            userInfo.Gender = UserGender;
            db.SaveChanges();
            ResetState();
        }

        [ObservableProperty]
        private BitmapImage _imageSource;

        private void LoadImageFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ImageSource = null;
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(url, UriKind.Absolute);
                    bitmap.EndInit();
                    ImageSource = bitmap;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }


    }
}
