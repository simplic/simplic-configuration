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
        /// <param name="configurationName">Configuration name</param>
        /// <param name="configurationValue">Configuration value</param>
        public void SetValue(string pluginName, string userName, string configurationName, string configurationValue)
        {
            // Wenn es eine Benutzerabhängige Konfiguration ist und noch kein Wert existiert, dann versuchen wir den datensatz zu kopieren.
            if (string.IsNullOrWhiteSpace(userName) == false)
            {
                if (GetValue(pluginName, userName, configurationName) == null)
                {
                    var sql = $"INSERT INTO {TableName}(PlugInName, UserName, ConfigName, ConfigValue, ContentType, " +
                        $" IsEditable, UserCanOverwrite) " +
                        $" (SELECT PlugInName, :userName, ConfigName, :configurationValue, ContentType, IsEditable, " +
                        $" UserCanOverwrite FROM {TableName} WHERE PlugInName = :pluginName AND " +
                        $" ConfigName = :configurationName AND UserName = '')";

                    sqlService.OpenConnection((connection) =>
                    {

                        var affectedRows = connection.Execute(sql,
                            new { pluginName, userName, configurationName, configurationValue });

                        return affectedRows > 0;
                    });
                }
            }

            if (GetValue(pluginName, userName, configurationName) == null)
            {
                sqlService.OpenConnection((connection) =>
                {
                    var sql = $"INSERT INTO {TableName}(PlugInName, UserName, ConfigName, ConfigValue) " +
                        $" values(:pluginName, :userName, :configurationName, :configurationValue)";

                    var affectedRows = connection.Execute(sql,
                           new { pluginName, userName, configurationName, configurationValue });

                    return affectedRows > 0;
                });
            }
            else
            {
                sqlService.OpenConnection((connection) =>
                {
                    var sql = $"UPDATE {TableName} SET ConfigValue = :configurationValue WHERE PlugInName = :pluginName " +
                            $" AND UserName = :userName AND ConfigName = :configurationName";

                    var affectedRows = connection.Execute(sql,
                           new { pluginName, userName, configurationName, configurationValue });

                    return affectedRows > 0;
                });
            }
        }

        /// <summary>
        /// Create a new configuration entry
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="type">Type (0 = string, 1 = int, 5 = bool)</param>
        /// <param name="editable">Determines whether the configuration is editable</param>
        /// <param name="configurationValue">Configuration value</param>
        public void Create(string configurationName, string pluginName, int type, bool editable, string configurationValue)
        {
            sqlService.OpenConnection((connection) =>
            {
                var sql = $"INSERT INTO {TableName} (PlugInName, UserName, ConfigName, ConfigValue, IsEditable, ContentType) " +
                    $" values(:pluginName, '', :configurationName, :configurationValue, :isEditable, :contentType)";

                connection.Execute(sql, new
                {
                    pluginName,
                    configurationName,
                    configurationValue,
                    isEditable = editable,
                    contentType = type
                });
            });
        }

        /// <summary>
        /// Gets a list configuration values
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="plugInName">Plugin name</param>
        /// <param name="userName">User name</param>
        /// <returns>A list configuration values</returns>
        public IEnumerable<ConfigurationValue> GetValues(string plugInName, string userName)
        {
            var rawValues = sqlService.OpenConnection((connection) =>
            {
                return connection.Query($"SELECT ConfigValue, ConfigName FROM {TableName} " +
                     $" WHERE PlugInName = :plugInName AND UserName = :userName", new { plugInName, userName });
            });

            foreach (var rawValue in rawValues)
            {
                yield return new ConfigurationValue(rawValue.ConfigName, plugInName, userName, rawValue.ConfigValue);
            }
        }

        /// <summary>
        /// Checks whether a configuration exists
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="pluginName">Plugin name</param>
        public bool Exists(string configurationName, string pluginName)
        {
            var sql = $"SELECT COUNT(*) FROM {TableName} WHERE PlugInName LIKE :pluginName AND ConfigName LIKE :configurationName ";

            return sqlService.OpenConnection((connection) =>
            {
                return connection.Query<int>(sql, new { pluginName, configurationName }).FirstOrDefault() > 0;
            });
        }
    }
}
