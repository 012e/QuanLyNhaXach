using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DocumentFormat.OpenXml.Drawing;
using System.Windows.Documents;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.LoginUI.Services;
using System.Collections.ObjectModel;
using BookstoreManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;
using BookstoreManagement.Shared.CustomMessages;
using ToastNotifications.Core;

namespace BookstoreManagement.SettingUI
{
    public partial class AllNotificationVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly CurrentUserService currentUserService;

        [ObservableProperty]
        private ObservableCollection<Note> _noteList;

        public AllNotificationVM(ApplicationDbContext db,
            CurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
        }

        [ObservableProperty]
        private GridLength _widthDetailNote;
        [ObservableProperty]
        private GridLength _widthListNote;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _content;

        [ObservableProperty]
        private DateTime _createAt;

        [ObservableProperty]
        private DateOnly _dueDate;

        [ObservableProperty]
        private string _createByName;

        public override void ResetState()
        {
            base.ResetState();
            LoadItem();
        }
        private async Task LoadItem()
        {
            DefaultValue();
            // Load List Note
            var noteList = await db.Notes
                .Include(n => n.Employee)
                .ToListAsync();

            // Construct NoteList
            NoteList = new ObservableCollection<Note>(
                noteList.Select(n => new Note
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    CreatedAt = n.CreatedAt,
                    DueDate = n.DueDate,
                    Employee = n.Employee
                }) 
             );
        }

        private void DefaultValue()
        {
            WidthListNote = new GridLength(1, GridUnitType.Star);
            WidthDetailNote = new GridLength(0, GridUnitType.Star);
            CreateAt = DateTime.Now;
            DueDate = DateOnly.FromDateTime(DateTime.Now);
        }

        private async void CreateNewNote()
        {
            try
            {
                var newNote = new Note
                {
                    Title = Title,
                    Content = Content,
                    CreatedAt = CreateAt,
                    DueDate = DueDate,
                    Employee = currentUserService.CurrentUser
                };
                db.Notes.Add(newNote);
                await db.SaveChangesAsync();
                await LoadItem();
            }
            catch(Exception ex)
            {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void OpenNewNote()
        {
            WidthListNote = new GridLength(1, GridUnitType.Star);
            WidthDetailNote = new GridLength(0.8, GridUnitType.Star);
        }

        [RelayCommand]
        private void Submit()
        {
            CreateNewNote();
        }

        [RelayCommand]
        private void CloseNewNote()
        {
            WidthListNote = new GridLength(1, GridUnitType.Star);
            WidthDetailNote = new GridLength(0, GridUnitType.Star);
        }

        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Create note successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void ErrorNotification()
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", "Create note failure", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }

    }
}
