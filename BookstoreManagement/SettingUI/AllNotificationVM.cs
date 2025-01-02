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
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using DocumentFormat.OpenXml.Wordprocessing;
using static ClosedXML.Excel.XLPredefinedFormat;

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
        private System.DateTime _createAt = System.DateTime.Now;

        [ObservableProperty]
        private System.DateTime _dueDate;

        [ObservableProperty]
        private string _createByName;

        [ObservableProperty]
        private bool _toggled;

        [ObservableProperty]
        private bool _createOrEditIsOpen;

        [ObservableProperty]
        private string _searchText;

        private async void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadItem().ConfigureAwait(false);
                return;
            }
            var filterNote = db.Notes
                .Include(n => n.Employee)
                .Where(n => n.Title.ToLower().Contains(SearchText.ToLower()))
                .ToList();
            NoteList = LoadNote(filterNote);
        }

        // Load note
        private ObservableCollection<Note> LoadNote(IEnumerable<Note> notes)
        {
            return new ObservableCollection<Note>(notes.Select(n => new Note
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                DueDate = n.DueDate,
                Employee = n.Employee,
                CreatedAt = n.CreatedAt
            }));
        }

        public override void ResetState()
        {
            base.ResetState();
            Task.Run(LoadItem);
        }
        private async Task LoadItem()
        {
            CloseNewNote();
            SetDefaultValue();

            var noteList = await db.Notes
                .Include(n => n.Employee)
                .ToListAsync();

            NoteList = LoadNote(noteList);
        }

        partial void OnSearchTextChanged(string value)
        {
            Search();
        }

        private void SetDefaultValue()
        {
            CreateAt = System.DateTime.Now;
            DueDate = System.DateTime.Now;
            Title = "";
            CreateByName = currentUserService.CurrentUser.FirstName + " " + currentUserService.CurrentUser.LastName;
            Content = "";
        }

        private async Task CreateNewNote()
        {

            var newNote = new Note
            {
                Title = Title,
                Content = Content,
                CreatedAt = CreateAt,
                DueDate = DateOnly.FromDateTime(DueDate),
                EmployeeId = currentUserService.CurrentUser!.Id
            };
            if(CreateAt > DueDate)
            {
                throw new Exception("Error");
            }
            db.Notes.Add(newNote);
            await db.SaveChangesAsync();
            await LoadItem();
            SetDefaultValue();
        }

       
        [RelayCommand]
        private void OpenNewNote()
        {
            CreateOrEditIsOpen = true;
            if (!Toggled)
            {
                Toggled = true;
                WidthListNote = new GridLength(1, GridUnitType.Star);
                WidthDetailNote = new GridLength(0.8, GridUnitType.Star);
                SetDefaultValue();
            }
            else
            {
                CloseNewNote();
                Toggled = false;
            }
        }

        [RelayCommand]
        private async Task Submit()
        {
            if (CreateOrEditIsOpen)
            {
                try
                {
                    await CreateNewNote();
                    SuccessNotification("Create note successfully");
                }
                catch (Exception ex)
                {
                    ErrorNotification("Due day must langer create at");
                }
            }
            else
            {
                SaveEdit();
                SuccessNotification("Save note successfully");
            }

        }
        [RelayCommand]
        private void CloseNewNote()
        {
            CreateOrEditIsOpen = false;
            WidthListNote = new GridLength(1, GridUnitType.Star);
            WidthDetailNote = new GridLength(0, GridUnitType.Star);
            Toggled = false;
        }

        private void SuccessNotification(string content)
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", content, NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void ErrorNotification(string content)
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", content, NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void WarningNotification(string content)
        {
            GetNotification.NotifierInstance.WarningMessage("Warning", content, NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }

        // ================================ Section for Edit =================================
        [ObservableProperty]
        private Note _selectedNote;


        // Delete Note
        [RelayCommand]
        private async Task DeleteNote(Note note)
        {
            WarningNotification("This action can be undone");
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this note?", "Delete note", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            
            db.Notes.Where(i => i.Id == note.Id).ExecuteDelete();
            await db.SaveChangesAsync();
            await LoadItem();
            SuccessNotification("Delete note successfully");
        }

        // Button to open edit

        [RelayCommand]
        private void ButtonEdit()
        {
            WarningNotification("This action can be undone");
            WidthListNote = new GridLength(1, GridUnitType.Star);
            WidthDetailNote = new GridLength(0.8, GridUnitType.Star);
            EditNote();
        }

        // Binding data to display
        private void EditNote()
        {
            if (SelectedNote != null)
            {
                Title = SelectedNote.Title;
                Content = SelectedNote.Content;
                CreateAt = SelectedNote.CreatedAt;
                DueDate = SelectedNote.DueDate.ToDateTime(TimeOnly.MinValue);
                CreateByName = $"{SelectedNote.Employee.FirstName} {SelectedNote.Employee.LastName}";
            }
        }

        // Save edit changed
        private async void SaveEdit()
        {
            var note = await db.Notes.Where(i => i.Id == SelectedNote.Id).FirstAsync();
            note.Title = Title;
            note.Content = Content;
            note.CreatedAt = CreateAt;
            note.DueDate = DateOnly.FromDateTime(DueDate);
            db.Update(note);
            await db.SaveChangesAsync();
            await LoadItem();
        }

        // ================================ End Section for Edit =================================


    }
}
