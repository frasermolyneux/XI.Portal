namespace XI.Portal.Data.Core.Migrations
{
    using System.Data.Entity.Migrations;
    using XI.Portal.Data.Core.Context;

    internal sealed class Configuration : DbMigrationsConfiguration<PortalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PortalContext context)
        {

        }
    }
}
