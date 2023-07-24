using Simplic.Cache;
using System;
using System.Collections.Generic;

namespace Simplic.Configuration.Service
{
    /// <inheritdoc/>
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
        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IEnumerable<ConfigurationValue> GetValues<T>(string pluginName, string userName)
        {
            foreach (var config in configurationRepository.GetValues(pluginName, userName))
            {
                config.Value = CastConfigurationValue<T>(config.Value);
                yield return config;
            }
        }

        /// <inheritdoc/>
        public void SetValue<T>(string configurationName, string pluginName, string userName, T value)
        {
            object raw = value;

            if (raw is bool || (raw is bool? && raw != null))
            {
                raw = Convert.ToInt32(raw);
            }

            string str = raw?.ToString() ?? "";

            configurationRepository.SetValue(pluginName, userName, configurationName, str);

            var configValue = cacheService.Get<ConfigurationValue>(
                ConfigurationValue.GetKeyName(configurationName, pluginName, userName));

            if (configValue != null)
                configValue.Value = str;
            else
                cacheService.Set(new ConfigurationValue(configurationName, pluginName, userName, str));
        }

        /// <inheritdoc/>
        public void Create<T>(string configurationName, string pluginName, int type, bool editable, T configurationValue)
        {
            configurationRepository.Create(configurationName, pluginName, type, editable, "");
            SetValue<T>(configurationName, pluginName, "", configurationValue);
        }

        /// <inheritdoc/>
        public bool Exists(string configurationName, string pluginName) => configurationRepository.Exists(configurationName, pluginName);
    }
}
