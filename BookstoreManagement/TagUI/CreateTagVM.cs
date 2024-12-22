using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Windows;

namespace BookstoreManagement.UI.TagUI
{
    public partial class CreateTagVM : BaseViewModel
    {
        private readonly ApplicationDbContext Db;
        private readonly INavigatorService<AllTagsVM> AllTagNavigator;

        [ObservableProperty]
        private Tag _tag;
        [ObservableProperty]
        private string _errorMessage = string.Empty;
        public CreateTagVM(ApplicationDbContext db, INavigatorService<AllTagsVM> allTagNavigator)
        {
            Db = db;
            AllTagNavigator = allTagNavigator;
        }
        [RelayCommand]
        private void NavigateBack()
        {
            AllTagNavigator.Navigate();
        }
        [RelayCommand]
        private void Submit()
        {
            if (!Check_Valid_Input())
            {
                MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                Db.Add(Tag);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add tag : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Added tag successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private bool Check_Valid_Input()
        {
            bool containsNumberInName = Tag.Name.Any(char.IsDigit);
            if (string.IsNullOrWhiteSpace(Tag.Name))
            {
                ErrorMessage = "Tag name can not be empty!";
                return false;
            }
            if (containsNumberInName)
            {
                ErrorMessage = "Tag name can not have number!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Tag.Description))
            {
                ErrorMessage = "Tag description can not be empty!";
            }
            bool containsNumberInDescription = Tag.Description.Any(char.IsDigit);
            if (containsNumberInDescription)
            {
                ErrorMessage = "Tag description can not have numnber!";
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
        private void ResetToDefaultValues()
        {
            Tag = new Tag
            {
                Name = "",
                Description = ""
            };
        }
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }
    }
}
