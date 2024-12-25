using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
        private string _userRollText;

        [ObservableProperty]
        private string _userGenderText;

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

                UserGenderText = userInfo.Gender ? "Male" : "Female";

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
                UserAddress= string.Empty;
                UserPhone = string.Empty;
                UserGenderText= "Unknown";
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
            userInfo.Address = UserAddress;
            db.SaveChanges();
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

        [ObservableProperty]
        private string _newProfilePicture;

        [RelayCommand]        
        private void ImportImage()
        {
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = db.Employees.FirstOrDefault(e => e.Id == userId);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files|*.jpg;*.png";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
            {
                NewProfilePicture = GetConvertPathToUrl(openFileDialog.FileName);
                LoadImageFromUrl(NewProfilePicture);
                userInfo.ProfilePicture = NewProfilePicture;
                db.SaveChanges();
                ResetState();
            }
        }

        private string GetConvertPathToUrl(string filePath)
        {
            return new Uri(filePath).AbsoluteUri;
            
        }
    }
}
