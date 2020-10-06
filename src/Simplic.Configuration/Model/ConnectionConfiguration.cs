namespace Simplic.Configuration
{
    /// <summary>
    /// Poco to represent a connection
    /// </summary>
    public class ConnectionConfiguration
    {
        /// <summary>
        /// Gets or sets the Id (DB column ID)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the tenant name (DB column mnd_name)
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the tenant number (DB column mnd_nummer)
        /// </summary>
        public int TenantNumber { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection type (DB column dbtype)
        /// </summary>
        public int ConnectionType { get; set; }
    }
}