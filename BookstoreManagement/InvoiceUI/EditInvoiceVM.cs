using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.InvoiceUI.Dtos;
using BookstoreManagement.InvoiceUI.Exporters;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using ToastNotifications.Core;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class EditInvoiceVM : EditItemVM<Invoice>
{
    private readonly ApplicationDbContext db;
    private readonly PricingService pricingService;
    private readonly InvoiceExporter invoiceExporter;


    // Day la danh sach chua cac item co trong hoa don
    [ObservableProperty]
    private ObservableCollection<InvoiceItemDto> _invoiceItemDto = new();

    public INavigatorService<AllInvoicesVM> AllInvoicesNavigator { get; }

    [ObservableProperty]
    private Invoice _invoice;

    [ObservableProperty]
    private Customer _selectedCutomer;

    [ObservableProperty]
    private bool _isSet = false;

    [ObservableProperty]
    private ObservableCollection<Customer> _customerList;

    [ObservableProperty]
    private String _searchText = "";

    [ObservableProperty]
    private int _customerId;

    [ObservableProperty]
    private ObservableCollection<Item> _allItems = [];

    [RelayCommand]
    private void GoBack()
    {
        AllInvoicesNavigator.Navigate();
    }

    public override void ResetState()
    {
        base.ResetState();
        IsInvoiceItemVisible = false;
    }

    protected override void LoadItem()
    {
        Invoice = db.Invoices.Find(ViewModelContext.Id)!;
        var itemsFromInvoice = (from invoiceItems in db.InvoicesItems
                                join items in db.Items on invoiceItems.ItemId equals items.Id
                                join itemPrice in db.ItemPrices on items.Id equals itemPrice.Id
                                where invoiceItems.InvoiceId == Invoice.Id
                                select new InvoiceItemDto
                                {
                                    id = items.Id,
                                    Name = items.Name,
                                    Quantity = invoiceItems.Quantity,
                                    Price = pricingService.GetPrice(items.Id).FinalPrice,
                                    TotalPrice = pricingService.GetPrice(items.Id).FinalPrice * invoiceItems.Quantity
                                });
        CustomerId = Invoice.CustomerId;
        InvoiceItemDto = new ObservableCollection<InvoiceItemDto>(itemsFromInvoice);
        var customers = db.Customers.OrderBy(i => i.Id).ToList();
        CustomerList = new ObservableCollection<Customer>(customers);
        AllItems = new ObservableCollection<Item>(db.Items.OrderBy(i => i.Id).ToList());
        this.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(SearchText))
            {
                UpdateFilter();
            }
        };
        PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(SelectedCutomer))
            {

                IsSet = SelectedCutomer != null ? false : true;
            }
        };

    }

    [RelayCommand]
    private void OpenSetCustomer()
    {
        IsSet = true;
    }

    [RelayCommand]
    private void CloseSetCustomer()
    {
        IsSet = false;
    }

    private void UpdateFilter()
    {
        var customers = db.Customers.OrderBy(i => i.Id).ToList();
        var query = string.IsNullOrWhiteSpace(SearchText)
            ? new ObservableCollection<Customer>(customers)
        : new ObservableCollection<Customer>(
                customers.Where(i => i.FirstName.ToLower().Contains(SearchText.ToLower()) ||
                i.LastName.ToLower().Contains(SearchText.ToLower()) ||
                i.PhoneNumber.ToString().ToLower().Contains(SearchText.ToLower())));

        CustomerList = query;
    }
    partial void OnSelectedCutomerChanged(Customer? oldValue, Customer newValue)
    {
        if (newValue is null)
        {
            return;
        }
        CustomerId = newValue.Id;
    }

    public EditInvoiceVM(ApplicationDbContext db,
        INavigatorService<AllInvoicesVM> allInvoicesNavigator,
        PricingService pricingService,
        InvoiceExporter invoiceExporter)
    {
        this.db = db;
        this.pricingService = pricingService;
        this.invoiceExporter = invoiceExporter;
        AllInvoicesNavigator = allInvoicesNavigator;
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
        if (Quantity > 0)
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
                id = row.Cell(1).GetValue<int>(),
                Quantity = row.Cell(2).GetValue<int>(),
                Price = row.Cell(3).GetValue<decimal>()

            });
        }
    }

    // Add Item Command
    [RelayCommand]
    private void AddItemIntoInvoiceItem()
    {
        if (Quantity <= 0)
        {
            ErrorNotificationQuantity();
            return;
        }

        var itemIntoInvoiceItem = new InvoicesItem()
        {
            ItemId = ItemId,
            InvoiceId = Invoice.Id,
            Quantity = Quantity
        };
        try
        {
            // Check Item is existed in InvoiceItemDto
            bool check = false;
            foreach (var item in InvoiceItemDto)
            {
                // If find => add quantity
                if (item.id == ItemId)
                {
                    item.Quantity += itemIntoInvoiceItem.Quantity;
                    check = true;
                    break;
                }
            }
            // else = not find Item want add in InvoiceItemDto
            // Add new Item
            if (!check)
            {
                decimal price = pricingService.GetPrice(ItemId).FinalPrice;
                InvoiceItemDto.Add(new InvoiceItemDto
                {
                    id = itemIntoInvoiceItem.ItemId,
                    Name = db.Items.FirstOrDefault(i => i.Id == itemIntoInvoiceItem.ItemId)?.Name ?? "Unknow",
                    Quantity = itemIntoInvoiceItem.Quantity,
                    Price = price,
                    TotalPrice = itemIntoInvoiceItem.Quantity * price
                });
            }
            SuccessAddNotification();
        }
        catch (Exception ex)
        {
            ErrorDBNotification();
        }
    }


    // Edit InvoiceItem
    [RelayCommand]
    private void EditInvoiceItem()
    {
        if (SelectInvoiceItem == null)
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

            // Biding du lieu tu item duoc chon lenh o Input
            ItemId = SelectInvoiceItem.id;
            Quantity = SelectInvoiceItem.Quantity;
        }
    }
    //check input
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    private bool IsOnlyNumber(string input)
    {
        return Regex.IsMatch(input, @"\d");
    }
    private bool Check_Valid_Input()
    {
        if (string.IsNullOrWhiteSpace(Invoice.EmployeeId.ToString()))
        {
            ErrorMessage = "Employee Id can not be empty!";
            return false;
        }
        if (!IsOnlyNumber(Invoice.EmployeeId.ToString()))
        {
            ErrorMessage = "Employee Id must be a non-negative integer!";
            return false;
        }
        if (!db.Employees.Any(e => e.Id == Invoice.EmployeeId))
        {
            ErrorMessage = "Employee Id is not exists!";
            return false;
        }
        ErrorMessage = string.Empty;
        return true;

    }
    [ObservableProperty]
    private bool _isSubmitSuccess = false;
    protected override void SubmitItemHandler()
    {
        if(!Check_Valid_Input())
        {
            GetNotification.NotifierInstance.ErrorMessage("Warning", ErrorMessage, NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
            return;
        }
        SaveChange();
        IsSubmitSuccess = true;
    }
    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        if (IsSubmitSuccess)
        {
            SuccessSaveNotification();
            IsSubmitSuccess = false;
        }
        return;
    }
    private void SaveChange()
    {
        try
        {
            var existingItems = db.InvoicesItems.Where(ii => ii.InvoiceId == Invoice.Id).ToList();
            db.InvoicesItems.RemoveRange(existingItems);

            //Invoice.InvoicesItems = [];
            Invoice.InvoicesItems = new List<InvoicesItem>();

            // Tien hanh kiem tra
            decimal total = 0;
            foreach (var itemDto in InvoiceItemDto)
            {
                var newItem = new InvoicesItem
                {
                    InvoiceId = Invoice.Id,
                    ItemId = itemDto.id,
                    Quantity = itemDto.Quantity

                };
                total += itemDto.TotalPrice;
                Invoice.InvoicesItems.Add(newItem);
            }
            Invoice.Total = total;
            Invoice.CustomerId = CustomerId;
            db.Invoices.Update(Invoice);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            ErrorDBNotification();
        }
    }

    [RelayCommand]
    private void SaveEdit()
    {
        if (SelectInvoiceItem == null)
        {
            ErrorNotificationChooseItem();
            return;
        }
        WarningNotification();
        var result = MessageBox.Show("Are you sure you want to save changes?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            NotAllowEdit = false;
            IsIconSaveEdit = false;
            SelectInvoiceItem.Quantity = Quantity;
            SelectInvoiceItem.TotalPrice = SelectInvoiceItem.Quantity * SelectInvoiceItem.Price;
            SaveChange();
            LoadItem();
            ResetValue();
        }
    }

    // Delete Command
    [RelayCommand]
    private void DeleteInvoiceItem()
    {
        WarningNotification();
        var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            InvoiceItemDto.Remove(SelectInvoiceItem);
            SuccessDeleteNotification();
        }
    }
    // Toast notification
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
        GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted invoice successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void SuccessAddNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Add invoice successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void ErrorDBNotification()
    {
        GetNotification.NotifierInstance.ErrorMessage("Error", "Couldn't add item: Database Error", NotificationType.Error, new MessageOptions
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
        GetNotification.NotifierInstance.ErrorMessage("Error", "Please choose Item", NotificationType.Error, new MessageOptions
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
    // ========================= End Section Detail Item ===================================

    [RelayCommand]
    private async Task ExportPdf()
    {
        await invoiceExporter.ExportPdf(Invoice.Id);
    }
}
