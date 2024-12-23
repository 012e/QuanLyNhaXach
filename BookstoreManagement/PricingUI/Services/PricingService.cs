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

    private decimal GetConcreteValue(ItemPrice itemPrice) => (decimal)itemPrice.Value / itemPrice.Divider;

    public PricingResponseDto GetPrice(int itemId)
    {
        var item = db.Items
            .Include(i => i.ItemPrices)
            .ThenInclude(ip => ip.PriceTypeNavigation)
            .FirstOrDefault(i => i.Id == itemId)
            ?? throw new ArgumentNullException(nameof(itemId));

        var prices = item.ItemPrices;
        decimal basePrice = GetBasePrice(item, prices);

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

    private decimal GetBasePrice(Item item, ICollection<ItemPrice> prices)
    {
        var basePriceRaw = item.ItemPrices.FirstOrDefault(p => p.PriceType == 1) ??
            throw new InvalidOperationException($"Item with id {item.Id} doesn't have a base price");
        var basePrice = GetConcreteValue(basePriceRaw);
        return basePrice;
    }

    private List<PricingDetail> GetOtherPrices(ICollection<ItemPrice> prices)
    {
        var rawOtherPrices = prices.Where(p => p.PriceType != 1);
        List<PricingDetail> otherPrices = [];
        foreach (var otherPrice in rawOtherPrices)
        {
            var divider = otherPrice.Divider;
            var value = otherPrice.Value;
            PricingDetail pricingDetail = new()
            {
                Name = otherPrice.PriceTypeNavigation.Name,
                Percentage = GetConcreteValue(otherPrice)
            };
            otherPrices.Add(pricingDetail);
        }

        return otherPrices;
    }
}
