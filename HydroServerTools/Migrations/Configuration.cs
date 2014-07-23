namespace HydroServerTools.Migrations
{
    using HydroServerTools.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HydroServerTools.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(HydroServerTools.Models.ApplicationDbContext context)
        {
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
            //context.ConnectionParameters.AddOrUpdate(
            //    p => p.Id,
            //    new ConnectionParameters
            //    {
            //        Id=1,
            //        Name = "HydroServertest2",
            //        DataSource = "tcp:bhi5g2ajst.database.windows.net,1433",
            //        InitialCatalog = "HydroServertest2",
            //        UserId = "HisCentralAdmin@bhi5g2ajst",
            //        Password = "f@deratedResearch"
            //    },
            //        new ConnectionParameters
            //        {
            //            Id=2,                        
            //            Name = "HydroServertest1",
            //            DataSource = "tcp:bhi5g2ajst.database.windows.net,1433",
            //            InitialCatalog = "HydroServertest1",
            //            UserId = "HisCentralAdmin@bhi5g2ajst",
            //            Password = "f@deratedResearch"
            //        },
            //       new ConnectionParameters
            //       {
            //           Id=3,
            //           Name = "HydroServertest3",
            //           DataSource = "tcp:bhi5g2ajst.database.windows.net,1433",
            //           InitialCatalog = "HydroServertest3",
            //           UserId = "HisCentralAdmin@bhi5g2ajst",
            //           Password = "f@deratedResearch"
            //       }
            //    );
            context.Users.AddOrUpdate(
                p => p.Id,
                //new User
                //{
                //    Id="2a1c313c-75d4-4fc3-822d-3355b630d17f",
                //    UserName = "HydroServertest1@gmail.com",
                //    PasswordHash = "AIc46WDIg9A9mp7/EVpd8h7WRZHthyeNQ5AHY8K87522TL0IScBq1Tx/4G0pyuqBMA==",
                    
                //},
                //new User
                //{
                //    //Email = "HydroServertest2@gmail.com",
                //    UserName = "HydroServertest2@gmail.com",
                //    PasswordHash = "AIc46WDIg9A9mp7/EVpd8h7WRZHthyeNQ5AHY8K87522TL0IScBq1Tx/4G0pyuqBMA==",
                    
                //},
                new User
                {
                    //Email = "HydroServertest2@gmail.com",
                    UserName = "mseul@cuahsi.org",
                    PasswordHash = "AJI01zyWV54cK/Yv46+7IUOagSea7pASukVzv1IPi/AndJ4j9CD6hfuf3+flarSGdg==",
                    
                });
                //context.ConnectionParametersUser.AddOrUpdate(
                //    p=> p.Id,
                //    new ConnectionParametersUser
                //    {
                //        ConnectionParametersId = 1,
                //        UserId = "2a1c313c-75d4-4fc3-822d-3355b630d17f"
                //    }
               // );
            context.SaveChanges();
        }
    }
}
