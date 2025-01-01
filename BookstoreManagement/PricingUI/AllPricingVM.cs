using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.PricingUI.Dtos;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.PricingUI;

public partial class AllPricingVM(
    PricingService pricingService,
    ApplicationDbContext db,
    IContextualNavigatorService<EditPricingVM, PricingResponseDto> editItemNavigator,
    MailService mailService)
    : ListVM<PricingResponseDto, EditPricingVM>
{
    private readonly PricingService pricingService = pricingService;
    private readonly ApplicationDbContext db = db;
    private readonly MailService mailService = mailService;

    protected override IContextualNavigatorService<EditPricingVM, PricingResponseDto> EditItemNavigator => editItemNavigator;

    protected override void DeleteItem(PricingResponseDto item)
    {
        throw new NotImplementedException();
    }

    [ObservableProperty]
    private string _searchText;


    [RelayCommand]
    private void Mail()
    {
        mailService.SendAsync("huyphmnhat@gmail.com", "Hello wordl", "this is some content");
    }

    protected override void LoadItems()
    {
        Items = new(pricingService.GetAllPricing().ToList());
    }
}
