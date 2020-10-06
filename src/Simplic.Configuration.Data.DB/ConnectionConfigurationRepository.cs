using Dapper;
using Simplic.Log;
using Simplic.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simplic.Configuration.Data.DB
{
    /// <summary>
    /// Class to implement all connection configuration repository functions
    /// </summary>
    public class ConnectionConfigurationRepository : IConnectionConfigurationRepository
    {
        private readonly ISqlService sqlService;
        private readonly ISqlColumnService sqlColumnService;

        /// <summary>
        /// Initializes a new instance of ConnectionConfigurationRepository
        /// </summary>
        /// <param name="sqlService"></param>
        /// <param name="sqlColumnService"></param>
        public ConnectionConfigurationRepository(ISqlService sqlService, ISqlColumnService sqlColumnService)
        {
            this.sqlService = sqlService;
            this.sqlColumnService = sqlColumnService;
        }

        /// <summary>
        /// Deletes a ConnectionConfiguration
        /// </summary>
        /// <param name="obj">The object to delete</param>
        /// <returns>Wherther one connection configuration was deleted</returns>
        public bool Delete(ConnectionConfiguration obj)
        {
            return sqlService.OpenConnection((connection) =>
            {
                return connection.Execute($"Delete from {TableName} where ID = :Id", new { obj.Id }) == 1;
            });
        }

        /// <summary>
        /// Deletes a ConnectionConfiguration
        /// </summary>
        /// <param name="id">The id of the object to delete</param>
        /// <returns>Wherther one connection configuration was deleted</returns>
        public bool Delete(int id)
        {
            return sqlService.OpenConnection((connection) =>
            {
                return connection.Execute($"Delete from {TableName} where ID = :id", new { id }) == 1;
            });
        }

        /// <summary>
        /// Gets an connection configuration
        /// </summary>
        /// <param name="id">the id of the connection configuration</param>
        /// <returns>The connection configuration instance</returns>
        public ConnectionConfiguration Get(int id)
        {
            return sqlService.OpenConnection((connection) =>
            {
                return connection.Query<ConnectionConfiguration>($"Select *, ID as Id, mnd_name as TenantName, mnd_nummer as TenantNumber, dbtype as ConnectionType from {TableName} where ID = :id", new { id }).FirstOrDefault();
            });
        }

        /// <summary>
        /// Gets all connection configurations
        /// </summary>
        /// <returns>An enumerable of connection configuration instances</returns>
        public IEnumerable<ConnectionConfiguration> GetAll()
        {
            return sqlService.OpenConnection((connection) =>
            {
                return connection.Query<ConnectionConfiguration>($"Select *, ID as Id, mnd_name as TenantName, mnd_nummer as TenantNumber, dbtype as ConnectionType from {TableName}");
            });
        }

        /// <summary>
        /// Saves a connection configuration
        /// </summary>
        /// <param name="obj">The object to save</param>
        /// <returns>Wherther the object saved succesfully</returns>
        public bool Save(ConnectionConfiguration obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(ConnectionConfiguration));

            if (obj.Id == 0)
                obj.Id = sqlService.OpenConnection((connection) =>
                {
                    return connection.Query($"Select GetIdentity('{TableName}')").FirstOrDefault();
                });

            try
            {
                var diffColumns = new Dictionary<string, string>()
            {
                {"ID", "Id" },
                {"mnd_name", "TenantName" },
                {"mnd_nummer", "TenantNumber" },
                {"dbtype", "ConnectionType" }
            };

                var dbColumns = sqlColumnService.GetModelDBColumnNames(TableName, typeof(ConnectionConfiguration), diffColumns);

                sqlService.OpenConnection((connection) =>
                {
                    connection.Execute($"INSERT INTO {TableName} ({string.Join(", ", dbColumns.Select(item => item.Key))}) ON EXISTING UPDATE VALUES "
                        + $" ({string.Join(", ", dbColumns.Select(k => ":" + (string.IsNullOrWhiteSpace(k.Value) ? k.Key : k.Value)))});", obj);
                });
            }
            catch (Exception ex)
            {
                LogManagerInstance.Instance.Error("Error while trying to save ConnectionConfiguration in Simplic.Configuration.Data.DB", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets a conncetion configuration by the given tenant name
        /// </summary>
        /// <param name="name">The tenant name of the connection configuration</param>
        /// <returns>A connection configuration object</returns>
        public ConnectionConfiguration GetByName(string name)
        {
            return sqlService.OpenConnection((connection) =>
            {
                return connection.Query<ConnectionConfiguration>($"Select *, ID as Id, mnd_name as TenantName, mnd_nummer as TenantNumber, dbtype as ConnectionType from {TableName} where mnd_name = :name", new { name }).FirstOrDefault();
            });
        }

        /// <summary>
        /// Gest the table name
        /// </summary>
        public string TableName
        {
            get => "ESS_DC_BASE_DB_Connection";
        }
    }
}