using BookstoreManagement.Shared.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BookstoreManagement.PricingUI.Dtos;

public class PricingResponseDto
{
    public required Item Item { get; set; }
    public required decimal BasePrice { get; set; }
    public required List<PricingDetail> PricingDetails { get; set; } = [];
    public required decimal FinalPrice { get; set; }
}
