namespace Inzynierka_ostatnia.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Inzynierka_ostatnia.Models.KaczkaDBCtxt>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Inzynierka_ostatnia.Models.KaczkaDBCtxt";
        }

        protected override void Seed(Inzynierka_ostatnia.Models.KaczkaDBCtxt context)
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
        }
    }
}
