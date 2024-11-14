using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Models
{
    public partial class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public string Status { get; set; } = null!; 
        public decimal DiscountValue { get; set; }
        public int MaxUsage { get; set; }
        public int RemainingUsage { get; set; }

        public List<PromotionItem> PromotionItems { get; set; } = new List<PromotionItem>();
        public List<PromotionInvoice> PromotionInvoices { get; set; } = new List<PromotionInvoice>();

    }
}
