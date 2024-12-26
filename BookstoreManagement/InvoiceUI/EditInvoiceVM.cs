﻿using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.InvoiceUI.Dtos;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.DashboardUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Automation;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class EditInvoiceVM : EditItemVM<Invoice>
{
    private readonly ApplicationDbContext db;
    private readonly PricingService pricingService;


    // Day la danh sach chua cac item co trong hoa don
    [ObservableProperty]
    private ObservableCollection<InvoiceItemDto> _invoiceItemDto = new();

    public INavigatorService<AllInvoicesVM> AllInvoicesNavigator { get; }
    public IContextualNavigatorService<AddInvoiceItemVM, Invoice> AddInvoiceItemNavigator { get; }

    [ObservableProperty]
    private Invoice _invoice;


    [RelayCommand]
    private void GoBack()
    {
        AllInvoicesNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToAddInvoiceItem()
    {
        if (Invoice == null) return;
        AddInvoiceItemNavigator.Navigate(Invoice);
    }

    public override void ResetState()
    {
        base.ResetState();
        InvoiceItemDto = new(); // Lam moi lai danh sach = tao danh sach trong
        IsInvoiceItemVisible = false;
    }

    protected override void LoadItem()
    {
        Invoice = ViewModelContext;
        var itemsFromInvoice = (from invoiceItems in db.InvoicesItems
                                join items in db.Items on invoiceItems.ItemId equals items.Id
                                join itemPrice in db.ItemPrices on items.Id equals itemPrice.Id
                                where invoiceItems.InvoiceId == Invoice.Id
                                select new InvoiceItemDto
                                {
                                    id = items.Id,
                                    Name = items.Name,
                                    Quantity = invoiceItems.Quantity,
                                    Price = pricingService.GetPrice(items.Id).FinalPrice
                                });

        InvoiceItemDto = new ObservableCollection<InvoiceItemDto>(itemsFromInvoice);
    }

    protected override void SubmitItemHandler()
    {
        db.Invoices.Update(Invoice);
        db.SaveChanges();
    }

    public EditInvoiceVM(ApplicationDbContext db,
        INavigatorService<AllInvoicesVM> allInvoicesNavigator,
        IContextualNavigatorService<AddInvoiceItemVM, Invoice> addInvoiceItemNavigator,
        PricingService pricingService
        )
    {
        this.db = db;
        this.pricingService = pricingService;
        AllInvoicesNavigator = allInvoicesNavigator;
        AddInvoiceItemNavigator = addInvoiceItemNavigator;
    }

    // Button SelectItem in CreateImport
    [RelayCommand]
    private void SelectItem()
    {
        IsInvoiceItemVisible = true;
    }

    // ========================= Section Detail Item ===================================

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private int _quantity;

   

    // Visibility Detail Item
    [ObservableProperty]
    private bool _isInvoiceItemVisible;

    [ObservableProperty]
    private InvoiceItemDto _selectInvoiceItem;

    [ObservableProperty]
    private bool _isIconSaveEdit;

    [ObservableProperty]
    private bool _notAllowEdit;

    [RelayCommand]
    private void ArrowInvoiceItem()
    {
        ResetValue();
        IsInvoiceItemVisible = false;
    }

    private void ResetValue()
    {
        ItemId = 0;
        Quantity = 0;
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
        if (openFileDialog.ShowDialog() == true)
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
        InvoiceItemDto.Clear();
        foreach (var row in rows)
        {
            InvoiceItemDto.Add(new InvoiceItemDto
            {
                id  = row.Cell(1).GetValue<int>(),
                Quantity = row.Cell(2).GetValue<int>()
            });
        }
    }

    // Add Item Command
    [RelayCommand]
    private void AddItemIntoInvoiceItem()
    {
        if (Quantity <= 0)
        {
            MessageBox.Show("Quantity must larger than 0", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        // Tao ra de luu vao co so du lieu
        // Khong tao InvoiceItemDto vi cac do chi de hien thi thoi, con luu lai thi 
        // phai su dung InvoicesItem
        var itemIntoInvoiceItem = new InvoicesItem()
        {
            ItemId = ItemId,
            InvoiceId = Invoice.Id,
            Quantity = Quantity
        };
        try
        {
            // Neu nhu Id item them da co trong danh sach
            // thi chi can cap nhap so luong
            bool check = false;
            foreach (var item in InvoiceItemDto)
            {
                if (item.id == ItemId)
                {
                    item.Quantity += itemIntoInvoiceItem.Quantity;
                    check = true;
                    // Luu vao csdl

                    break;
                }
            }
            // Neu them item moi => them item nay vao 
            if (!check)
            {
                InvoiceItemDto.Add(new InvoiceItemDto
                {
                    id = itemIntoInvoiceItem.ItemId,
                    Name = db.Items.FirstOrDefault(i => i.Id == itemIntoInvoiceItem.ItemId)?.Name ?? "Unknow",
                    Quantity = itemIntoInvoiceItem.Quantity,
                    Price = pricingService.GetPrice(ItemId).FinalPrice
                });
            }
            MessageBox.Show("Added item successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could'n add item : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        ResetState();
    }


    // Edit ImportItem
    [RelayCommand]
    private void EditInvoiceItem()
    {
        if (SelectInvoiceItem == null)
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

            // Biding du lieu tu item duoc chon lenh o Input
            ItemId = SelectInvoiceItem.id;
            Quantity = SelectInvoiceItem.Quantity;
            
        }
    }

    [RelayCommand]
    private void SaveEdit()
    {
        if (SelectInvoiceItem == null)
        {
            MessageBox.Show("Please choose Item !", "Error",

                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var result = MessageBox.Show("Are you sure you want to save changes?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            NotAllowEdit = false;
            IsIconSaveEdit = false;

            try
            {
                // Xoa cac item cu trong CSDL
                var exisItem = db.InvoicesItems.Where(ii => ii.InvoiceId == Invoice.Id).ToList();
                db.InvoicesItems.RemoveRange(exisItem);

                // Them cac item tu Dto vao CSDL - InvoiceItems;
                foreach (var itemDto in InvoiceItemDto)
                {
                    var newItem = new InvoicesItem
                    {
                        InvoiceId = Invoice.Id,
                        ItemId = itemDto.id,
                        Quantity = itemDto.Quantity
                    };
                    db.InvoicesItems.Add(newItem);
                }
                db.SaveChanges();
                MessageBox.Show("Changes saved successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    // Delete Command
    [RelayCommand]
    private void DeleteInvoiceItem()
    {
        var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            InvoiceItemDto.Remove(SelectInvoiceItem);
            MessageBox.Show("Item removed from the list.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    // ========================= End Section Detail Item ===================================
}
