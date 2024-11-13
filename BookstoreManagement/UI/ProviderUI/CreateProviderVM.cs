using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookstoreManagement.UI.ProviderUI
{
    public partial class CreateProviderVM : BaseViewModel
    {
        private readonly ApplicationDbContext Db;
        private readonly INavigatorService<AllProviderVM> allProviderNavigator;

        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _address;

        public CreateProviderVM(ApplicationDbContext db , INavigatorService<AllProviderVM> AllProviderNavigator)
        {
            Db = db;
            allProviderNavigator = AllProviderNavigator;
        }
        [RelayCommand]
        private void NavigateBack()
        {
            allProviderNavigator.Navigate();
        }
        [RelayCommand]
        private void Submit()
        {
            var item = new Provider
            {
                Name = Name,
                Address = Address,
            };
            try
            {
                Db.Add(item);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add provider : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Added provider successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ResetToDefaultValues()
        {
            Name = string.Empty;
            Address = string.Empty;
        }
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }
    }
}
