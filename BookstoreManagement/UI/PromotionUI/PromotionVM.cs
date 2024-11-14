using BookstoreManagement.Core;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BookstoreManagement.UI.PromotionUI
{
    public partial class PromotionVM : BaseViewModel
    {
        

        public ObservableCollection<Promotion> Promotions { get; set; }

        [RelayCommand]
        private void OnOffButton(Promotion promotion)
        {

        }

        // Khai bao Navigator chuyen den CreatePromotion
        protected INavigatorService<CreatePromotionVM> CreatePromotionNavigator { get; }

        public PromotionVM(INavigatorService<CreatePromotionVM> createPromotionNavigator)
        {
            Promotions = GetSamplePromotions();
            this.CreatePromotionNavigator = createPromotionNavigator;
        }


        // Nut chuyen den Create
        [RelayCommand]
        private void NavigateToCreatePromotion()
        {
            CreatePromotionNavigator.Navigate();
        }

        // Du lieu mau
        private ObservableCollection<Promotion> GetSamplePromotions()
        {
            return new ObservableCollection<Promotion>
            {
                new Promotion
                {
                    Id = 1,
                    Name = "Giang Sinh",
                    Description = "Giam gia 10% cho tat ca san pham",
                    StartDay = DateTime.Now.AddDays(-10),
                    EndDay = DateTime.Now.AddDays(10),
                    Status = "Hoat dong",
                    DiscountValue = 10.00m,
                    MaxUsage = 100,
                    RemainingUsage = 100
                },
                 new Promotion
                {
                    Id = 2,
                    Name = "Tet Nguyen Dang",
                    Description = "Giam gia 20% cho tat ca san pham",
                    StartDay = DateTime.Now.AddDays(-2),
                    EndDay = DateTime.Now.AddDays(4),
                    Status = "Hoat dong",
                    DiscountValue = 20.00m,
                    MaxUsage = 20,
                    RemainingUsage = 20
                },
                  new Promotion
                {
                    Id = 3,
                    Name = "Huong Mua He",
                    Description = "Giam gia 15% cho tat ca san pham",
                    StartDay = DateTime.Now.AddDays(-3),
                    EndDay = DateTime.Now.AddDays(24),
                    Status = "Hoat dong",
                    DiscountValue = 15.00m,
                    MaxUsage = 30,
                    RemainingUsage = 30
                },
                   new Promotion
                {
                    Id = 4,
                    Name = "Mua Tuu Truong",
                    Description = "Giam gia 40% cho tat ca san pham",
                    StartDay = DateTime.Now.AddDays(-30),
                    EndDay = DateTime.Now.AddDays(30),
                    Status = "Hoat dong",
                    DiscountValue = 20.00m,
                    MaxUsage = 60,
                    RemainingUsage = 60
                }
            };
        }

    }

}
