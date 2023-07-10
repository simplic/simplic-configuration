using Dapper;
using Simplic.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simplic.Configuration.Data
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private const string TableName = "ESS_MS_Intern_Config";
        private readonly ISqlService sqlService;

        public ConfigurationRepository(ISqlService sqlService)
        {
            this.sqlService = sqlService;
        }

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="plugInName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <param name="configurationName">Configuration name</param>
        /// <returns>Configuration value</returns>
        public string GetValue(string pluginName, string userName, string configurationName)
        {
            var sql = $"SELECT ConfigValue FROM {TableName} WHERE " +
                $" PlugInName LIKE :pluginName and UserName LIKE :username and ConfigName LIKE :configurationName ";

            return sqlService.OpenConnection((connection) =>
            {
                return connection.Query<string>(sql, new { pluginName, userName, configurationName })
                    .FirstOrDefault();
            });
        }

        /// <summary>
        /// Sets a configuration value (saves in the db)
        /// </summary>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <param name="configName">Configuration name</param>
        /// <param name="configValue">Configuration value</param>
        public void SetValue(string pluginName, string userName, string configName, string configValue)
        {
            string sql;

            // Wenn es eine Benutzerabhängige Konfiguration ist und noch kein Wert existiert, dann versuchen wir den datensatz zu kopieren.
            if (!string.IsNullOrWhiteSpace(userName) && GetValue(pluginName, userName, configName) == null)
            {

                sql = $"INSERT INTO {TableName}(PlugInName, UserName, ConfigName, ConfigValue, ContentType, " +
                    $" IsEditable, UserCanOverwrite) " +
                    $" (SELECT PlugInName, :userName, ConfigName, :configValue, ContentType, IsEditable, " +
                    $" UserCanOverwrite FROM {TableName} WHERE PlugInName = :pluginName AND " +
                    $" ConfigName = :configName AND UserName = '')";
            }
            else
            {
                if (GetValue(pluginName, userName, configName) == null)
                {
                    sql = $"INSERT INTO {TableName} (PlugInName, UserName, ConfigName, ConfigValue) " +
                        $" values(:pluginName, :userName, :configName, :configValue)";
                }
                else
                {
                    sql = $"UPDATE {TableName} SET ConfigValue = :configValue WHERE PlugInName = :pluginName " +
                            $" AND UserName = :userName AND ConfigName = :configName";
                }
            }

            sqlService.OpenConnection((connection) =>
            {
                return connection.Execute(sql, new { pluginName, userName, configName, configValue }) > 0;
            });
        }

        /// <summary>
        /// Create a new configuration entry
        /// </summary>
        /// <param name="configName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="type">Type (0 = string, 1 = int, 5 = bool)</param>
        /// <param name="editable">Determines whether the configuration is editable</param>
        /// <param name="configValue">Configuration value</param>
        public void Create(string configName, string pluginName, int type, bool editable, string configValue)
        {
            sqlService.OpenConnection((connection) =>
            {
                connection.Execute($"INSERT INTO {TableName} (PlugInName, UserName, ConfigName, ConfigValue, IsEditable, ContentType) " +
                    $" values(:pluginName, '', :configName, :configValue, :editable, :type)",
                    new
                    {
                        pluginName,
                        configName,
                        configValue,
                        editable,
                        type
                    });
            });
        }

        /// <summary>
        /// Gets a list configuration values
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <returns>A list configuration values</returns>
        public IEnumerable<ConfigurationValue> GetValues(string pluginName, string userName)
        {
            var rawValues = sqlService.OpenConnection((connection) =>
            {
                return connection.Query($"SELECT ConfigValue, ConfigName FROM {TableName} " +
                     $" WHERE PlugInName = :pluginName AND UserName = :userName", new { pluginName, userName });
            });

            foreach (var rawValue in rawValues)
            {
                yield return new ConfigurationValue(rawValue.ConfigName, pluginName, userName, rawValue.ConfigValue);
            }
        }

        /// <summary>
        /// Checks whether a configuration exists
        /// </summary>
        /// <param name="configName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        public bool Exists(string configName, string pluginName)
        {
            return sqlService.OpenConnection((connection) =>
            {
                return connection.Query<int>($"SELECT COUNT(*) FROM {TableName}" +
                    $"WHERE PlugInName LIKE :pluginName AND ConfigName LIKE :configName ",
                    new { pluginName, configName }).FirstOrDefault() > 0;
            });
        }
    }
}
