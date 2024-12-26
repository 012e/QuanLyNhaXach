using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Supabase;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BookstoreManagement.SettingUI
{
    public partial class MyProfileVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly CurrentUserService currentUserService;
        private readonly ImageUploader imageBucket;

        public MyProfileVM(ApplicationDbContext db,
            CurrentUserService currentUserService,
            ImageUploader imageUploader)
        {
            this.db = db;
            this.currentUserService = currentUserService;
            this.imageBucket = imageUploader;
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
        [NotifyPropertyChangedFor(nameof(IsLoading))]
        private bool _isUploadingImage = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLoading))]
        private bool _isUpdatingImage = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLoading))]
        private bool _isLoadingInitialData = false;

        public bool IsLoading => IsUpdatingImage || IsUpdatingImage || IsLoadingInitialData;

        [ObservableProperty]
        private string _userPhone;
        public override void ResetState()
        {
            base.ResetState();
            LoadCurrentUser();
        }


        private async Task LoadCurrentUser()
        {
            IsLoadingInitialData = true;
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = await db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == userId);

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

                LoadImageFromUrl(imageBucket.GetPublicUrl(userInfo.ProfilePicture));
            }
            else
            {
                UserFirstName = string.Empty;
                UserLastName = string.Empty;
                UserEmail = string.Empty;
                UserRollText = "Unknown";
                UserAddress = string.Empty;
                UserPhone = string.Empty;
                UserGenderText = "Unknown";
            }
            IsLoadingInitialData = false;
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
            var userInfo = await db.Employees.FirstAsync(e => e.Id == userId);

            userInfo.FirstName = UserFirstName;
            userInfo.LastName = UserLastName;
            userInfo.PhoneNumber = UserPhone;
            userInfo.Email = UserEmail;
            userInfo.Birthday = UserBirthDay;
            userInfo.Address = UserAddress;
            db.SaveChanges();
            MessageBox.Show("Updated user informations successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    IsUpdatingImage = true;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(url, UriKind.Absolute);
                    bitmap.EndInit();
                    ImageSource = bitmap;
                    IsUpdatingImage = false;
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
        private async Task ImportImage()
        {
            var userId = currentUserService.CurrentUser.Id;
            var userInfo = await db.Employees.FirstAsync(e => e.Id == userId);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files|*.jpg;*.png";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
            {
                IsUploadingImage = true;
                NewProfilePicture = await imageBucket.ReplaceImageAsync(userInfo.ProfilePicture, openFileDialog.FileName);
                userInfo.ProfilePicture = NewProfilePicture;
                db.SaveChanges();
                MessageBox.Show("Profile picture uploaded sucessfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadImageFromUrl(imageBucket.GetPublicUrl(NewProfilePicture));
                IsUploadingImage = false;
            }
        }
    }
}
