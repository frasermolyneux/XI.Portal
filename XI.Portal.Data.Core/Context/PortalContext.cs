using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using XI.Portal.Data.Core.Migrations;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Data.Core.Context
{
    public class PortalContext : IdentityDbContext<ApplicationUser>
    {
        public PortalContext()
        {
            //Requires empty constructor
            Database.Connection.ConnectionString = ConnectionStringHack.ConnectionString;
        }

        public PortalContext(string connectionString) : base(connectionString)
        {
            ConnectionStringHack.ConnectionString = connectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PortalContext, PortalDbMigrationConfiguration>());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<GameServer> GameServers { get; set; }
        public DbSet<Demo> Demos { get; set; }
        public DbSet<LivePlayer> LivePlayers { get; set; }
        public DbSet<Player2> Players { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<FileMonitor> FileMonitors { get; set; }
        public DbSet<BanFileMonitor> BanFileMonitors { get; set; }
        public DbSet<RconMonitor> RconMonitors { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<MapFile> MapFiles { get; set; }
        public DbSet<MapRotation> MapRotations { get; set; }
        public DbSet<MapVote> MapVotes { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<AdminAction> AdminActions { get; set; }
        public DbSet<PlayerIpAddress> PlayerIpAddresses { get; set; }
        public DbSet<PlayerAlias> PlayerAliases { get; set; }
        public DbSet<LivePlayerLocation> LivePlayerLocations { get; set; }
    }
}