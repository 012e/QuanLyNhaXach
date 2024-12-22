using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace BookstoreManagement.UI.TagUI
{
    public partial class EditTagVM : EditItemVM<Tag>
    {
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private Tag _item;
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
            Item = db.Tags.Where(tag => tag.Id == ViewModelContext.Id).First();
        }
        private bool Check_Valid_Input()
        {
            if (string.IsNullOrWhiteSpace(Item.Name))
            {
                ErrorMessage = "Tag name can not be empty!";
                return false;
            }
            bool containsNumberInName = Item.Name.Any(char.IsDigit);
            if (containsNumberInName)
            {
                ErrorMessage = "Tag name can not have number!";
                return false;   
            }
            if (string.IsNullOrWhiteSpace(Item.Description))
            {
                ErrorMessage = "Tag description can not be empty!";
                return false;
            }
            bool containsNumberInDescription = Item.Description.Any(char.IsDigit);
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
                MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            db.Tags.Update(Item);
            db.SaveChanges();
            IsSubmitSuccess = true;
        }

        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            if (IsSubmitSuccess)
            {
                MessageBox.Show("Submitted successfully");
                IsSubmitSuccess = false;
            }
        }

        public EditTagVM(ApplicationDbContext db, INavigatorService<AllTagsVM> allTagsNavigator)
        {
            this.db = db;
            this.allTagsNavigator = allTagsNavigator;
            Item = new Tag();
        }
    }
}
