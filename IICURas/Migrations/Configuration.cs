namespace IICURas.Migrations
{
    using IICURas.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<IICURasContext>
    {
       

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(IICURasContext context)
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("IICURasConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }

            //context.Countries.AddOrUpdate(c => c.CountryName,
            //   new Country { CountryName = "Canada" },
            //   new Country { CountryName = "China" },
            //   new Country { CountryName = "France" },
            //   new Country { CountryName = "Germany" },
            //   new Country { CountryName = "Japan" },
            //   new Country { CountryName = "United Kingdom" },
            //   new Country { CountryName = "United States of America" },
            //   new Country { CountryName = "Other Countries" }
            //   );

            //context.Categories.AddOrUpdate(c => c.CategoryName,
            //    new Category { CategoryName = "Control" },
            //    new Category { CategoryName = "Intervention" }
            //    );
        }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
