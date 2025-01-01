using BookstoreManagement.Core;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using ToastNotifications.Core;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace BookstoreManagement.ImportUI
{
    public partial class CreateImportVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly INavigatorService<AllImportVM> allImportNavigator;

        [ObservableProperty]
        private bool _isImportItemVisible;

        [ObservableProperty]
        private ObservableCollection<ImportItem> _listItem;

        [ObservableProperty]
        private ObservableCollection<Provider> _providerList;

        [ObservableProperty]
        private int _providerId;

        [ObservableProperty]
        private decimal _totalCost;

        [ObservableProperty]
        private DateTime _createdAt;

        // Declare in ImportItemTab
        [ObservableProperty]
        private int _itemId;

        [ObservableProperty]
        private int _quantity;


        [ObservableProperty]
        private ImportItem _selectImportItem;

        [ObservableProperty]
        private bool isIconSaveEdit;

        // This use for update state of TextBoX ItemId not allow Edit when click button Edit
        [ObservableProperty]
        private bool _notAllowEdit;


        public CreateImportVM(ApplicationDbContext db,
            INavigatorService<AllImportVM> allimportNavigator)
        {
            this.db = db;
            this.allImportNavigator = allimportNavigator;
            ListItem = new ObservableCollection<ImportItem>();
            LoadProviderList();
        }

        // Load provider List
        private void LoadProviderList()
        {
            var providers = db.Providers.ToList();
            ProviderList = new ObservableCollection<Provider>(providers);
        }

        // Add Import Command
        [RelayCommand]
        private void Submit()
        {
            var import = new Import
            {
                ProviderId = ProviderId,
                CreatedAt = CreatedAt,
                ImportItems = ListItem,
                TotalCost = TotalCost
            };
            try
            {
                db.Add(import);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Could'n add Import : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SuccessAddNotification();
        }

        // Go Back Command
        [RelayCommand]
        private void GoBack()
        {
            allImportNavigator.Navigate();
        }

        // Reset Values
        private void ResetToDefaultValues()
        {
            _providerId = 0;
            _createdAt = DateTime.Now;
            _totalCost = 0;
            IsImportItemVisible = false;
        }

        // Update Status
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }

        // Button SelectItem in CreateImport
        [RelayCommand]
        private void SelectItem()
        {
            IsImportItemVisible = true;
        }       

        // ====================================== SECTION FOR IMPORT ITEM ===============================================

        // Button Close Tab ImportItem
        [RelayCommand]
        private void ArrowImportItem()
        {
            ResetValue(); // Reset value in Import Item
            IsImportItemVisible = false;
        }

        //  Add Item Command
        [RelayCommand]
        private void AddImportItem()
        {
            if (Quantity <= 0)
            {
                ErrorNotificationQuantity();
                return;
            }
            var importItem = new ImportItem
            {
                ItemId = ItemId,
                Quantity = Quantity
            };
            try
            {
                bool check = db.Items.Any(i => i.Id == importItem.ItemId);
                
                if (check)
                {
                    ListItem.Add(importItem);
                }
                ErrorDbNotification();
            }
            catch (Exception ex)
            {
                ErrorDbNotification();
            }
            ResetValue();
        }

        // Decrease quantity Button
        [RelayCommand]
        private void SubQuantity()
        {
            if(Quantity > 0)
            {
                Quantity--;
            }
        }

        // Increase quantity Button
        [RelayCommand]
        private void AddQuantity()
        {
            Quantity++;
        }


        // Import File Command
        [RelayCommand]
        private void ImportFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx",
                Title = "Select a file"
            };
            if(openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                LoadExcel(filePath);
            }
        }

        // Method Load Item from file
        private void LoadExcel(string filePath)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RowsUsed().Skip(1); // skip header line
            ListItem.Clear();
            foreach(var row in rows)
            {
                ListItem.Add(new ImportItem
                {
                    ItemId = row.Cell(1).GetValue<int>(),
                    Quantity = row.Cell(2).GetValue<int>()
                });
            }
        }

        // Reset ItemID and Quantity when close tab Import Item
        private void ResetValue()
        {
            ItemId = 0;
            Quantity = 0;
        }

        // Edit ImportItem
        [RelayCommand]
        private void EditImportItem()
        {
            if (SelectImportItem == null)
            {
                ErrorNotificationChooseItem();
                return;
            }
            WarningNotification();
            var result = MessageBox.Show("Are you sure you want to edit?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NotAllowEdit = true;
                IsIconSaveEdit = true;
                ItemId = SelectImportItem.ItemId;
                Quantity = SelectImportItem.Quantity;
            }
        }

        // SaveEdit Command
        [RelayCommand]
        private void SaveEdit()
        {
            if (SelectImportItem == null)
            {
                ErrorNotificationChooseItem();
                return;
            }
            WarningNotification();
            var result = MessageBox.Show("Are you sure you want to save changes?","Confirm",MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                NotAllowEdit = false;
                IsIconSaveEdit = false;
                SelectImportItem.Quantity = Quantity;
                SuccessSaveNotification();
            }
        }

        // Delete Command
        /// <summary>
        /// This just delete item from ListItem, not ImportItem(db)
        /// If we want delete from ImportItem, we will delete in EditImportVM
        /// </summary>
        [RelayCommand]
        private void DeleteImportItem()
        {
            WarningNotification();
            var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ListItem.Remove(SelectImportItem);
                SuccessDeleteNotification();
            }
        }
        // ===================================== END SECTION FOR IMPORT ITEM =============================================

        // Toast notification

        private void ErrorDbNotification()
        {
            GetNotification.NotifierInstance.WarningMessage("Error", "Couldn't add item: Database Error", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void WarningNotification()
        {
            GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }

        private void SuccessDeleteNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted import successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void SuccessAddNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Add import successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }

        private void SuccessSaveNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Save successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void ErrorNotificationChooseItem()
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", "Please choose Import", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void ErrorNotificationQuantity()
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", "Quantity must langer than 0", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
    }
}
