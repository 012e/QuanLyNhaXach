using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.PricingUI.Dto;

public class PricingResponseDto
{
    public decimal BasePrice { get; set; }
    public List<PricingDetail> PricingDetails { get; set; } = [];
    public decimal FinalPrice { get; set; }
}
