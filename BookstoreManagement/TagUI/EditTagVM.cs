using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;
using ToastNotifications.Core;

namespace BookstoreManagement.UI.TagUI
{
    public partial class EditTagVM : EditItemVM<Tag>
    {
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private Tag _tag;
        [ObservableProperty]
        private string _errorMessage = string.Empty;
        [ObservableProperty]
        private bool _isSubmitSuccess = false;  
        private readonly INavigatorService<AllTagsVM> allTagsNavigator;

        [RelayCommand]
        private void NavigateBack()
        {
            allTagsNavigator.Navigate();
        }

        protected override void LoadItem()
        {
            Tag = db.Tags.Where(tag => tag.Id == ViewModelContext.Id).First();
        }
        private bool Check_Valid_Input()
        {
            if (string.IsNullOrWhiteSpace(Tag.Name))
            {
                ErrorMessage = "Tag name can not be empty!";
                return false;
            }
            bool containsNumberInName = Tag.Name.Any(char.IsDigit);
            if (containsNumberInName)
            {
                ErrorMessage = "Tag name can not have number!";
                return false;   
            }
            if (string.IsNullOrWhiteSpace(Tag.Description))
            {
                ErrorMessage = "Tag description can not be empty!";
                return false;
            }
            bool containsNumberInDescription = Tag.Description.Any(char.IsDigit);
            if (containsNumberInDescription)
            {
                ErrorMessage = "Tag description can not have number!";
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
        protected override void SubmitItemHandler()
        {
            if (!Check_Valid_Input())
            {
                ErrorNotification();
                return;
            }
            db.Tags.Update(Tag);
            db.SaveChanges();
            IsSubmitSuccess = true;
        }

        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            if (IsSubmitSuccess)
            {
                SuccessNotification();
                IsSubmitSuccess = false;
            }
        }

        public EditTagVM(ApplicationDbContext db, INavigatorService<AllTagsVM> allTagsNavigator)
        {
            this.db = db;
            this.allTagsNavigator = allTagsNavigator;
            Tag = new Tag();
        }

        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Submit tag successfully", NotificationType.Error, new MessageOptions
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
