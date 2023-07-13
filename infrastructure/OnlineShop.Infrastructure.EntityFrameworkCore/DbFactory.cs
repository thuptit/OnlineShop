using Microsoft.EntityFrameworkCore;
using OnlineShop.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.EntityFrameworkCore
{
    public class DbFactory<TDbcontext> : IDbFactory<TDbcontext> where TDbcontext : DbContext
    {
        public TDbcontext CreateDbContext(string dbType, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDbcontext>();
            var type = ConvertDbType(dbType);
            switch (type)
            {
                case DbType.SQLSERVER:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case DbType.POSTGRESQL:
                    optionsBuilder.UseNpgsql(connectionString);
                    break;
                case DbType.ORACLE:
                    break;
                default:
                    throw new ArgumentException($"Not Implement Type: {type.ToString()}");
            }

            var contextType = typeof(TDbcontext);
            var instance = Activator.CreateInstance(contextType, new object[] {optionsBuilder.Options});

            return (TDbcontext)instance;
        }

        private DbType ConvertDbType(string type) => (DbType)Enum.Parse(typeof(DbType), type, true);

        public TDbcontext CreateDbContext(string[] args)
        {
            throw new NotImplementedException();
            var builder = new DbContextOptionsBuilder<TDbcontext>();

            /*
             You can provide an environmentName parameter to the AppConfigurations.Get method. 
             In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
             Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
             https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
             */
            //var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            //TestDbContextConfigurer.Configure(builder, configuration.GetConnectionString(TestConsts.ConnectionStringName));
        }
    }
}
