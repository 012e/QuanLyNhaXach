using BookstoreManagement.PricingUI.Dtos;
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

        var prices = item.ItemPrices.ToList();
        decimal basePrice = item.BasePrice;

        List<PricingDetail> otherPrices = GetOtherPrices(prices);

        decimal finalPrice = CalculateFinalPrice(basePrice, otherPrices);

        return new()
        {
            Item = item,
            BasePrice = basePrice,
            PricingDetails = otherPrices,
            FinalPrice = finalPrice
        };
    }

    private static decimal CalculateFinalPrice(decimal basePrice, List<PricingDetail> otherPrices)
    {
        var finalPrice = basePrice;
        for (int i = 0; i < otherPrices.Count; i++)
        {
            finalPrice += finalPrice * otherPrices[i].Percentage;
        }

        return finalPrice;
    }

    public IEnumerable<PricingResponseDto> GetAllPricing()
    {
        var itemsWithPrices = db.Items
            .Include(i => i.ItemPrices)
            .ToList();

        if (!itemsWithPrices.Any())
        {
            return [];
        }

        var pricingResponses = itemsWithPrices.Select(item =>
        {
            var prices = item.ItemPrices.ToList();
            decimal basePrice = item.BasePrice;

            List<PricingDetail> otherPrices = GetOtherPrices(prices);

            decimal finalPrice = CalculateFinalPrice(basePrice, otherPrices);

            return new PricingResponseDto
            {
                Item = item,
                BasePrice = basePrice,
                PricingDetails = otherPrices,
                FinalPrice = finalPrice
            };
        });

        return pricingResponses;
    }

    public PricingResponseDto GetPrice(Item item)
    {
        return GetPrice(item.Id);
    }

    private List<PricingDetail> GetOtherPrices(List<ItemPrice> prices)
    {
        List<PricingDetail> otherPrices = [];
        foreach (var otherPrice in prices.OrderBy(i => i.Ordering))
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

    public bool SavePrice(UpdatePricingRequestDto dto)
    {
        var item = db.Items
            .Include(i => i.ItemPrices)
            .FirstOrDefault(i => i.Id == dto.ItemId);

        if (item == null)
        {
            return false;
        }

        item.BasePrice = dto.BasePrice;

        var prices = item.ItemPrices;
        foreach (var price in prices)
        {
            db.ItemPrices.Remove(price);
        }

        for (var i = 0; i < dto.PricingDetails.Count; i++)
        {
            var pricingDetail = dto.PricingDetails[i];
            var itemPrice = new ItemPrice
            {
                PriceType = pricingDetail.Name,
                Percentage = pricingDetail.Percentage,
                Item = item,
                Ordering = i

            };

            db.ItemPrices.Add(itemPrice);
        }

        db.SaveChanges();

        return true;
    }
}
