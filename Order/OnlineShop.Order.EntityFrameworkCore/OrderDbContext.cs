using Microsoft.EntityFrameworkCore;
using OnlineShop.Infrastructure.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Order.EntityFrameworkCore
{
    public class OrderDbContext : EfCoreDbContext
    {
        public DbSet<OnlineShop.Order.EntityFrameworkCore.Entities.Order> Orders { get; set; }
        public OrderDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
