using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Models
{
    public partial class PromotionInvoice
    {
        public int PromotionId { get; set; }
        public int InvoiceId { get; set; }
        public string Conditon { get; set; } = null!;
        public virtual Promotion Promotion { get; set; } = null!;
        public virtual Invoice Invoice { get; set; } = null!;
    }
}
