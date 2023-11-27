using System;
using System.Collections.Generic;

namespace UniqueProducts.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? ClientId { get; set; }
        public int? ProductId { get; set; }
        public int? OrderAmount { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsCompleted { get; set; }
        public int? EmployeeId { get; set; }

        public virtual Client? Client { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual Product? Product { get; set; }
    }
}
