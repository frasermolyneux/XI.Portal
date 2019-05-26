namespace XI.Portal.Library.Rcon.Models
{
    public class RconPlayer
    {
        public string Num { get; set; }
        public string Score { get; set; }
        public string Ping { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string LastMsg { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string QueryPort { get; set; }
        public string Rate { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Name) &&
                               !string.IsNullOrWhiteSpace(Guid) &&
                               !string.IsNullOrWhiteSpace(IpAddress);
    }
}