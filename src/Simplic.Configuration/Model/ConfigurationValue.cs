﻿using Simplic.Cache;

namespace Simplic.Configuration
{
    /// <summary>
    /// ConfigurationValue
    /// </summary>
    public class ConfigurationValue : ICacheObject
    {
        private string configName;
        private string pluginName;
        private string userName;
        private object value;

        /// <summary>
        /// Constructor to create a new configuration value
        /// </summary>
        /// <param name="configName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <param name="value">Configuration value</param>
        public ConfigurationValue(string configName, string pluginName, string userName, object value)
        {
            this.configName = configName;
            this.pluginName = pluginName;
            this.userName = userName;
            this.value = value;
        }


        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }


        /// <summary>
        /// Gets the configuration name
        /// </summary>
        public string ConfigName
        {
            get
            {
                return configName;
            }
        }

        public string CacheKey
        {
            get { return GetKeyName(configName, pluginName, userName).ToLower().Trim(); }
        }


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