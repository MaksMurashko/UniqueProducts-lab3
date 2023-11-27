using System;
using System.Collections.Generic;

namespace UniqueProducts.Models
{
    public partial class Client
    {
        public Client()
        {
            Orders = new HashSet<Order>();
        }

        public int ClientId { get; set; }
        public string? Company { get; set; }
        public string? Representative { get; set; }
        public string? Phone { get; set; }
        public string? CompanyAddress { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
