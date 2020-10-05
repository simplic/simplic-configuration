using System.Collections.Generic;

namespace Simplic.Configuration.Service
{
    /// <summary>
    /// Class for all connection configuration service functions
    /// </summary>
    public class ConnectionConfigurationService : IConnectionConfigurationService
    {
        private readonly IConnectionConfigurationRepository connectionConfigurationRepository;

        public ConnectionConfigurationService(IConnectionConfigurationRepository connectionConfigurationRepository)
        {
            this.connectionConfigurationRepository = connectionConfigurationRepository;
        }

        /// <summary>
        /// Deletes a ConnectionConfiguration
        /// </summary>
        /// <param name="id">The id of the object to delete</param>
        /// <returns>Wherther one connection configuration was deleted</returns>
        public bool Delete(ConnectionConfiguration obj) => connectionConfigurationRepository.Delete(obj);

        /// <summary>
        /// Deletes a ConnectionConfiguration
        /// </summary>
        /// <param name="id">The id of the object to delete</param>
        /// <returns>Wherther one connection configuration was deleted</returns>
        public bool Delete(int id) => connectionConfigurationRepository.Delete(id);

        /// <summary>
        /// Gets an connection configuration
        /// </summary>
        /// <param name="id">the id of the connection configuration</param>
        /// <returns>The connection configuration instance</returns>
        public ConnectionConfiguration Get(int id) => connectionConfigurationRepository.Get(id);

        /// <summary>
        /// Gets all connection configurations
        /// </summary>
        /// <returns>An enumerable of connection configuration instances</returns>
        public IEnumerable<ConnectionConfiguration> GetAll() => connectionConfigurationRepository.GetAll();

        /// <summary>
        /// Saves a connection configuration
        /// </summary>
        /// <param name="obj">The object to save</param>
        /// <returns>Wherther the object saved succesfully</returns>
        public bool Save(ConnectionConfiguration obj) => connectionConfigurationRepository.Save(obj);
    }
}