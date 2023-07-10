using Simplic.Cache;
using System;
using System.Collections.Generic;

namespace Simplic.Configuration.Service
{
    public class ConfigurationService : IConfigurationService
    {
        #region Private Members
        private readonly ICacheService cacheService;
        private readonly IConfigurationRepository configurationRepository;
        #endregion

        public ConfigurationService(ICacheService cacheService, IConfigurationRepository configurationRepository)
        {
            this.cacheService = cacheService;
            this.configurationRepository = configurationRepository;
        }

        #region Private Methods

        #region [CastConfigurationValue]
        /// <summary>
        /// Casts a configuration value to a specific type
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="value">Value to cast</param>
        /// <returns>Casted value</returns>
        private T CastConfigurationValue<T>(object value)
        {
            if (value is T t)
                return t;

            if (typeof(T) == typeof(bool))
                value = Convert.ToInt32(value?.ToString());
            if (typeof(T) == typeof(bool?))
                value = value == null ? (int?)null : Convert.ToInt32(value.ToString());

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Gibt einen Konfigurationswert zurück
        /// </summary>
        /// <param name="configurationName">Konfigurationswert</param>
        /// <param name="pluginName">PlugInName</param>
        /// <param name="userName">Benutzername</param>
        /// <param name="noCaching">Wenn true, wird kein Cache verwendet</param>
        /// <returns>Wert</returns>
        public T GetValue<T>(string configurationName, string pluginName, string userName, bool noCaching = false)
        {
            if (!noCaching)
            {
                var returnValue = cacheService.Get<ConfigurationValue>(
                    ConfigurationValue.GetKeyName(configurationName, pluginName, userName));

                if (returnValue != null)
                    return CastConfigurationValue<T>(returnValue.Value);
            }

            var value = configurationRepository.GetValue(pluginName, userName, configurationName);

            // If no configuration value exists, try to load a user independent setting
            if (string.IsNullOrWhiteSpace(value))
                value = configurationRepository.GetValue(pluginName, "", configurationName);

            var configValue = new ConfigurationValue(configurationName, pluginName, userName, value);

            if (!noCaching)
                cacheService.Set(configValue);

            return CastConfigurationValue<T>(value);
        }

        /// <summary>
        /// Get an enumerable of configuration values by its plugin name
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="plugInName">PlugIn-Name</param>
        /// <param name="userName">Current username, should be empty for ignoring</param>
        /// <returns>Enumerable of values</returns>
        public IEnumerable<ConfigurationValue> GetValues<T>(string pluginName, string userName)
        {
            foreach(var config in configurationRepository.GetValues(pluginName, userName))
            {
                config.Value = CastConfigurationValue<T>(config.Value);
                yield return config;
            }
        }

        /// <summary>
        /// Setzt einen Konfigurationswert
        /// </summary>
        /// <param name="configurationName">Name der Konfiguration</param>
        /// <param name="pluginName">PlugIn-Name</param>
        /// <param name="userName">Benutzername</param>
        /// <param name="value">Wert</param>
        public void SetValue<T>(string configurationName, string pluginName, string userName, T value)
        {
            string str = null;

            if (value is bool)
            {
                str = Convert.ToInt32(value).ToString();
            }
            else if (value is bool? && value != null)
            {
                str = Convert.ToInt32(value).ToString();
            }

            configurationRepository.SetValue(pluginName, userName, configurationName, str);

            var configValue = cacheService.Get<ConfigurationValue>(
                ConfigurationValue.GetKeyName(configurationName, pluginName, userName));

            if (configValue != null)
                configValue.Value = str;
            else
                cacheService.Set(new ConfigurationValue(configurationName, pluginName, userName, str));
        }

        /// <summary>
        /// Create a new configuration entry
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="type">Type (0 = string, 1 = int, 5 = bool)</param>
        /// <param name="editable">Determines whether the configuration is editable</param>
        /// <param name="configurationValue">Configuration value</param>
        public void Create<T>(string configurationName, string pluginName, int type, bool editable, T configurationValue)
        {
            configurationRepository.Create(configurationName, pluginName, type, editable, "");
            SetValue<T>(configurationName, pluginName, "", configurationValue);
        }

        /// <summary>
        /// Checks whether a configuration exists
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        public bool Exists(string configurationName, string pluginName) => configurationRepository.Exists(configurationName, pluginName);
    }
}
