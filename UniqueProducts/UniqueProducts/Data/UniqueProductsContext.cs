using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UniqueProducts.Models;

namespace UniqueProducts.Data
{
    public partial class UniqueProductsContext : DbContext
    {
        public UniqueProductsContext(DbContextOptions<UniqueProductsContext> options) : base(options) { }

        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
    }
}
