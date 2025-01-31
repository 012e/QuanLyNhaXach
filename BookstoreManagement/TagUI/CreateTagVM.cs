﻿using BookstoreManagement.Core;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Windows;
using ToastNotifications.Core;

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
            try
            {
                if (!Check_Valid_Input())
                {
                    ErrorNotification();
                    return;
                }
                Db.Add(Tag);
                Db.SaveChanges();
                SuccessNotification();
                AllTagNavigator.Navigate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add tag : Database Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
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
        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Added tag successfully", NotificationType.Error, new MessageOptions
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
