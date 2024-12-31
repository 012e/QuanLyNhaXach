using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.PricingUI.Dtos;

public class UpdatePricingRequestDto
{
    public required int ItemId { get; set; }
    public required decimal BasePrice { get; set; }
    public required List<PricingDetail> PricingDetails { get; set; }
}
