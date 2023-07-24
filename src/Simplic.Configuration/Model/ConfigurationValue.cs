using Simplic.Cache;

namespace Simplic.Configuration
{
    /// <summary>
    /// ConfigurationValue
    /// </summary>
    public class ConfigurationValue : ICacheObject
    {
        private readonly string configName;
        private readonly string pluginName;
        private readonly string userName;
        private object value;

        /// <summary>
        /// Constructor to create a new configuration value
        /// </summary>
        /// <param name="configName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <param name="value">Configuration value</param>
        public ConfigurationValue(string configName, string pluginName, string userName, string value)
        {
            this.configName = configName;
            this.pluginName = pluginName;
            this.userName = userName;
            this.value = value;
        }


        public object Value { get => value; set => this.value = value; }


        /// <summary>
        /// Gets the configuration name
        /// </summary>
        public string ConfigName => configName;

        /// <summary>
        /// Gets the key used as a unique identifier in the cache
        /// </summary>
        public string CacheKey => GetKeyName(configName, pluginName, userName).ToLower().Trim();

        /// <summary>
        /// Generiert den eindeutigen Key eines ConvifurationValue - ICacheable
        /// </summary>
        /// <param name="configName">Configuration name</param>
        /// <param name="plugInName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <returns>Key name</returns>
        public static string GetKeyName(string configurationName, string plugInName, string userName)
        {
            return (configurationName + plugInName + userName).ToLower().Trim();
        }
    }
}