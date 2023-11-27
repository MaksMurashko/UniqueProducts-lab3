using System;
using System.Collections.Generic;

namespace UniqueProducts.Models
{
    public partial class Product
    {
        public Product()
        {
            Orders = new HashSet<Order>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescript { get; set; }
        public float? ProductWeight { get; set; }
        public float? ProductDiameter { get; set; }
        public string? ProductColor { get; set; }
        public int? MaterialId { get; set; }
        public decimal? ProductPrice { get; set; }

        public virtual Material? Material { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
