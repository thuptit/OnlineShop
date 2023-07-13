using Autofac;
using Microsoft.Extensions.Configuration;
using OnlineShop.Infrastructure.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Order.EntityFrameworkCore
{
    public class OrderEfCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(options =>
            {
                var config = options.Resolve<IConfigurationRoot>();
                var dbcontextFactory = new DbFactory<OrderDbContext>();
                var context = dbcontextFactory.CreateDbContext(config.GetConnectionString("DbType"), config.GetConnectionString("DefaultConnection"));
                return context;
            }).InstancePerLifetimeScope();
        }
    }
}
