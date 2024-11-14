using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Models
{
    public partial class PromotionItem
    {
        public int PromotionId { get; set; }
        public int ItemId { get; set; }
        public string Condition { get; set; } = null!;

        public virtual Promotion Promotion { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
    }
}
