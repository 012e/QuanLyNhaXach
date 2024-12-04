using BookstoreManagement.Core;
using BookstoreManagement.Core.Interface;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using BookstoreManagement.UI.ItemUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.UI.PromotionUI
{
    public partial class CreatePromotionVM : BaseViewModel
    {
        // Khai bao Navigator tro ve PromotionVM
        private readonly INavigatorService<PromotionVM> promotionNavigator;
        private readonly IAbstractFactory<SelectItemsVM> selectItemFactory;

        [ObservableProperty]
        private bool _isModalOpen = false;

        [ObservableProperty]
        private BaseViewModel _currentViewModel;


        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTime _startDay;

        [ObservableProperty]
        private DateTime _endDay;

        [ObservableProperty]
        private string _status;

        [ObservableProperty]
        private decimal _discountValue;

        [ObservableProperty]
        private int _maxUsage;

        [ObservableProperty]
        private int _remainingUsage;

        [ObservableProperty]
        private List<PromotionItem> promotionItems;

        //[ObservableProperty]
        //private List<PromotionInvoice> _promotionInvoices;


        // cap nhap trang thai
        public override void ResetState()
        {
            base.ResetState();
            IsModalOpen = false;
        }

        // Go back
        [RelayCommand]
        private void GoBack()
        {
            promotionNavigator.Navigate();
        }

        // Nut chuyen toi chon vat pham de ap dung khuyen mai
        [RelayCommand]
        private void SelectItems()
        {
            IsModalOpen = true;
        }

        public CreatePromotionVM(
            INavigatorService<PromotionVM> promotionNavigator,
            IAbstractFactory<SelectItemsVM> selectItemFactory)
            
        {
            this.promotionNavigator = promotionNavigator;
            this.selectItemFactory = selectItemFactory;
            this.CurrentViewModel = selectItemFactory.Create();
        }     
    }
}
