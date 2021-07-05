using System.Collections.Generic;

namespace Simplic.Configuration
{
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="plugInName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <param name="configurationName">Configuration name</param>
        /// <returns>Configuration value</returns>
        string GetValue(string pluginName, string userName, string configurationName);
        
        /// <summary>
        /// Sets a configuration value (saves in the db)
        /// </summary>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="configurationValue">Configuration value</param>
        void SetValue(string pluginName, string userName, string configurationName, string configurationValue);

        /// <summary>
        /// Create a new configuration entry
        /// </summary>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="type">Type (0 = string, 1 = int, 5 = bool)</param>
        /// <param name="editable">Determines whether the configuration is editable</param>
        /// <param name="configurationValue">Configuration value</param>
        void Create(string pluginName, string configurationName, int type, bool editable, string configurationValue);

        /// <summary>
        /// Gets a list configuration values
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="plugInName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <returns>A list configuration values</returns>
        IEnumerable<ConfigurationValue> GetValues<T>(string plugInName, string userName);
    }
}
