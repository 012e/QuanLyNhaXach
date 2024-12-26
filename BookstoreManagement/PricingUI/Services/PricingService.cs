using BookstoreManagement.PricingUI.Dto;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.PricingUI.Services;

public class PricingService(ApplicationDbContext db)
{
    private readonly ApplicationDbContext db = db;

    public PricingResponseDto GetPrice(int itemId)
    {
        var item = db.Items
            .Include(i => i.ItemPrices)
            .FirstOrDefault(i => i.Id == itemId)
            ?? throw new ArgumentNullException(nameof(itemId));

        var prices = item.ItemPrices;
        decimal basePrice = item.BasePrice;

        List<PricingDetail> otherPrices = GetOtherPrices(prices);

        var finalPrice = basePrice;
        for (int i = 0; i < otherPrices.Count; i++)
        {
            finalPrice += finalPrice * otherPrices[i].Percentage;
        }

        return new()
        {
            BasePrice = basePrice,
            PricingDetails = otherPrices,
            FinalPrice = finalPrice
        };
    }

    public PricingResponseDto GetPrice(Item item)
    {
        return GetPrice(item.Id);
    }

    private List<PricingDetail> GetOtherPrices(ICollection<ItemPrice> prices)
    {
        List<PricingDetail> otherPrices = [];
        foreach (var otherPrice in prices)
        {
            PricingDetail pricingDetail = new()
            {
                Name = otherPrice.PriceType,
                Percentage = otherPrice.Percentage,
            };
            otherPrices.Add(pricingDetail);
        }

        return otherPrices;
    }
}
