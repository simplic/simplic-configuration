using Simplic.Data;

namespace Simplic.Configuration
{
    /// <summary>
    /// Interface for connection configuration repository functions
    /// </summary>
    public interface IConnectionConfigurationRepository : IRepositoryBase<int, ConnectionConfiguration>
    {

        /// <summary>
        /// Gets a conncetion configuration by the given tenant name
        /// </summary>
        /// <param name="name">The tenant name of the connection configuration</param>
        /// <returns>A connection configuration object</returns>
        ConnectionConfiguration GetByName(string name);
    }
}