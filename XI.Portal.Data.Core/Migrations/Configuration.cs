using System.Data.Entity.Migrations;
using XI.Portal.Data.Core.Context;

namespace XI.Portal.Data.Core.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<PortalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "XI.Portal.Data.PortalContext";
        }
    }
}