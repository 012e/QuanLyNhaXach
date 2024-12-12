using BookstoreManagement.Core;
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
                ImportItems = ListItem
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
            MessageBox.Show("Added Import successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Quantity must larger than 0", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var importItem = new ImportItem
            {
                ItemId = ItemId,
                Quantity = Quantity
            };
            try
            {
                bool check = false;
                foreach(var item in ListItem)
                {
                    if(item.ItemId == importItem.ItemId)
                    {
                        item.Quantity += importItem.Quantity;
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    ListItem.Add(importItem);
                }
                MessageBox.Show("Added item successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add item : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Please choose Item !", "Error",

                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
                MessageBox.Show("Please choose Item !", "Error",

                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show("Are you sure you want to save changes?","Confirm",MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                NotAllowEdit = false;
                IsIconSaveEdit = false;
                SelectImportItem.Quantity = Quantity;
                MessageBox.Show("Update success", "Edit", MessageBoxButton.OK);
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
            var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ListItem.Remove(SelectImportItem);
            }
        }
        // ===================================== END SECTION FOR IMPORT ITEM =============================================
    }
}
