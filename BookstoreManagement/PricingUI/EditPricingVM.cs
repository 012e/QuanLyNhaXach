using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.PricingUI.Dtos;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ToastNotifications.Core;
using ToastNotifications.Messages.Error;

namespace BookstoreManagement.PricingUI;


public partial class CalculatedPricingDetail : ObservableObject, IDataErrorInfo
{
    public required string Name { get; set; }

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            if (columnName == nameof(Name))
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return "Name cannot be empty or blank.";
                }
            }

            return string.Empty; // No error
        }
    }


    [ObservableProperty]
    public decimal _percentage;

    [ObservableProperty]
    public decimal _value;
}

public partial class EditPricingVM : EditItemVM<PricingResponseDto>
{
    private readonly INavigatorService<AllPricingVM> allPricingNavigator;
    private readonly PricingService pricingService;

    public EditPricingVM(INavigatorService<AllPricingVM> allPricingNavigator, PricingService pricingService)
    {
        this.allPricingNavigator = allPricingNavigator;
        this.pricingService = pricingService;
        PrepareObservablCollection();
    }

    private void PrepareObservablCollection()
    {
        PricingDetails.CollectionChanged += (sender, args) =>
        {
            if (args.OldItems != null)
            {
                foreach (CalculatedPricingDetail oldItem in args.OldItems)
                {
                    oldItem.PropertyChanged -= (sender, args) =>
                    {
                        UpdatePricingDetail();
                    };

                }
            }

            if (args.NewItems != null)
            {
                foreach (CalculatedPricingDetail newItem in args.NewItems)
                {
                    newItem.PropertyChanged += (sender, args) =>
                    {
                        UpdatePricingDetail();
                    };
                }
            }
        };
    }

    [ObservableProperty]
    public decimal _finalPrice;

    private void UpdateFinalPrice()
    {
        decimal finalPrice = BasePrice;
        foreach (var detail in PricingDetails)
        {
            finalPrice += finalPrice * detail.Percentage;
        }
        FinalPrice = finalPrice;
    }

    [ObservableProperty]
    private Item _item;

    [ObservableProperty]
    private decimal _basePrice;

    partial void OnBasePriceChanged(decimal value)
    {
        UpdatePricingDetail();
    }

    partial void OnPricingDetailsChanging(ObservableCollection<CalculatedPricingDetail>? oldValue, ObservableCollection<CalculatedPricingDetail> newValue)
    {
        UpdatePricingDetail();
    }


    [ObservableProperty]
    private ObservableCollection<CalculatedPricingDetail> _pricingDetails = [];

    [RelayCommand]
    private void NavigateBack()
    {
        allPricingNavigator.Navigate();
    }

    public override void ResetState()
    {
        base.ResetState();
        PricingDetails.Clear();
    }

    protected override void LoadItem()
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            Item = ViewModelContext.Item;
            BasePrice = ViewModelContext.BasePrice;

            foreach (var detail in ViewModelContext.PricingDetails)
            {
                PricingDetails.Add(new()
                {
                    Name = detail.Name,
                    Percentage = detail.Percentage,
                    Value = 0
                });
            }

            UpdatePricingDetail();
        });
    }


    [RelayCommand]
    private void UpdatePricingDetail()
    {
        var currentPrice = BasePrice;
        for (int i = 0; i < PricingDetails.Count; i++)
        {
            CalculatedPricingDetail pricingDetail = PricingDetails[i];
            pricingDetail.Value = currentPrice * pricingDetail.Percentage;
            currentPrice += currentPrice * pricingDetail.Percentage;
        }
        FinalPrice = currentPrice;
    }

    private void SuccessNotification(string msg)
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", msg, NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }

    private void ErrorNotification(string msg)
    {
        GetNotification.NotifierInstance.ErrorMessage("Error", msg, NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }

    protected override void SubmitItemHandler()
    {
        var pricingDetails = new List<PricingDetail>();
        foreach (var detail in PricingDetails)
        {
            if (string.IsNullOrWhiteSpace(detail.Name))
            {
                throw new Exception("Name cannot be empty or blank.");
            }
            if (detail.Percentage <= 0)
            {
                throw new Exception("Percentage must be greater than 0.");
            }
            pricingDetails.Add(new PricingDetail
            {
                Name = detail.Name,
                Percentage = detail.Percentage
            });
        }

        var request = new UpdatePricingRequestDto
        {
            BasePrice = BasePrice,
            ItemId = Item.Id,
            PricingDetails = pricingDetails,
        };
        pricingService.SavePrice(request);
    }

    protected override void HandleSubmittingException(Exception e)
    {
        ErrorNotification($"Couldn't save: {e.Message}");
    }

    protected override void OnSubmittingSuccess()
    {
        SuccessNotification("Updated pricing successfully");
    }
}
