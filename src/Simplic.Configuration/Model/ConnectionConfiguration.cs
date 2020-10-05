namespace Simplic.Configuration
{
    /// <summary>
    /// Poco to represent a connection
    /// </summary>
    public class ConnectionConfiguration
    {
        public int Id { get; set; }

        public string TenantName { get; set; }

        public int TenantNumber { get; set; }

        public string ConnectionString { get; set; }

        public int ConnectionType { get; set; }
    }
}