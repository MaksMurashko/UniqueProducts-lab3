using System;
using System.Collections.Generic;

namespace UniqueProducts.Models
{
    public partial class Material
    {
        public Material()
        {
            Products = new HashSet<Product>();
        }

        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialDescript { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
