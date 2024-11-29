using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.TagUI
{
    public partial class CreateTagVM : BaseViewModel
    {
        private readonly ApplicationDbContext Db;
        private readonly INavigatorService<AllTagsVM> AllTagNavigator;

        [ObservableProperty]
        private String _name;

        [ObservableProperty]
        private String _description;

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
            var item = new Tag
            {
                Name = Name,
                Description = Description,
            };
            try
            {
                Db.Add(item);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add tag : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Added tag successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ResetToDefaultValues()
        {
            _name = string.Empty;
            _description = string.Empty;
        }
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }
    }
}
