namespace XI.Portal.Data.Core.Migrations
{
    using System.Data.Entity.Migrations;
    using XI.Portal.Data.Core.Context;

    internal sealed class PortalDbMigrationConfiguration : DbMigrationsConfiguration<PortalContext>
    {
        public PortalDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            ContextKey = "XI.Portal.Data.Core.Context";
        }

        protected override void Seed(PortalContext context)
        {

        }
    }
}
