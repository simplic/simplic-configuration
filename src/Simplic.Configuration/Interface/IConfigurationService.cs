using System.Collections.Generic;

namespace Simplic.Configuration
{
    /// <summary>
    /// Gets and sets configuration values
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Get an enumerable of configuration values by its plugin name
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="pluginName">Name of the plugin</param>
        /// <param name="userName">Name of the user. Uses user-independent setting if empty</param>
        /// <returns>Enumerable of values casted to <typeparamref name="T"/></returns>
        IEnumerable<ConfigurationValue> GetValues<T>(string pluginName, string userName);

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="configurationName">Name of the setting</param>
        /// <param name="pluginName">Name of the plugin</param>
        /// <param name="userName">Name of the user. Uses user-independent setting if empty</param>
        /// <param name="noCaching">If true, the value will not be loaded from the cache nor stored.</param>
        /// <returns>Configuration value casted to <typeparamref name="T"/></returns>
        T GetValue<T>(string configurationName, string pluginName, string userName, bool noCaching = false);

        /// <summary>
        /// Sets a configuration
        /// </summary>
        /// <typeparam name="T">Input type</typeparam>
        /// <param name="configurationName">Name of the configuration</param>
        /// <param name="pluginName">Name of the plugin</param>
        /// <param name="userName">Name of the user. Uses user-independent setting if empty</param>
        /// <param name="value">Configuration value</param>
        void SetValue<T>(string configurationName, string pluginName, string userName, T value);

        /// <summary>
        /// Checks whether a configuration exists
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        bool Exists(string configurationName, string pluginName);

        /// <summary>
        /// Create a new configuration entry
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="type">Type (0 = string, 1 = int, 5 = bool)</param>
        /// <param name="editable">Determines whether the configuration is editable</param>
        /// <param name="configurationValue">Configuration value</param>
        void Create<T>(string configurationName, string pluginName, int type, bool editable, T configurationValue);
    }
}
