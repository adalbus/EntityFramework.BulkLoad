namespace EntityFramework.BulkLoad
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class EntityBulkLoader// : IDataLoader
    {
        #region Fields

        /// <summary>
        ///     The connection string.
        /// </summary>
        private readonly string connectionString;

        #endregion

        #region Constructors and Destructors

        public EntityBulkLoader(string connectionString)
            : this(connectionString, new EntityBulkLoadSettings())
        {
        }

        public EntityBulkLoader(string connectionString, EntityBulkLoadSettings settings)
        {
            this.connectionString = connectionString;
            this.Settings = settings;
        }

        #endregion

        #region Public Properties

        public EntityBulkLoadSettings Settings { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void BulkInsert<T>(IEnumerable<T> items, DbContext context)
        {
            this.BulkInsert(items, null, context);
        }

        public void BulkInsert<T>(IEnumerable<T> items, string tableName, DbContext context)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(T).Name;
            }

            using (SqlBulkCopy bulkCopy = this.CreateSqlBulkCopyInstance(tableName))
            {
                IDataReader dataReader = items.AsDataReader(context);

                try
                {
                    bulkCopy.WriteToServer(dataReader);
                }
                finally
                {
                    bulkCopy.Close();
                }
            }
        }

        public void BulkInsert<T>(IEnumerable<T> items) where T : class
        {
            this.BulkInsert(items, null, null);
        }

        /// <summary>
        ///     Efficiently bulk insert the items from the collection into the destination set.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task BulkInsertAsync<T>(IEnumerable<T> items) where T : class
        {
            await this.BulkInsertAsync(items, null);
        }

        /// <summary>
        ///     The bulk insert.
        /// </summary>
        /// <typeparam name="T">
        ///     The entity type
        /// </typeparam>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="tableName">
        ///     Name of the table.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     items
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     Only SQL Server connections are supported.
        /// </exception>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task BulkInsertAsync<T>(IEnumerable<T> items, string tableName)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(T).Name;
            }

            using (SqlBulkCopy bulkCopy = this.CreateSqlBulkCopyInstance(tableName))
            {
                IDataReader dataReader = items.AsDataReader();

                try
                {
                    await bulkCopy.WriteToServerAsync(dataReader);
                }
                finally
                {
                    bulkCopy.Close();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The create sql bulk copy.
        /// </summary>
        /// <param name="tableName">
        ///     The table name.
        /// </param>
        /// <returns>
        ///     The <see cref="SqlBulkCopy" />.
        /// </returns>
        private SqlBulkCopy CreateSqlBulkCopyInstance(string tableName)
        {
            var bulkCopy = new SqlBulkCopy(this.connectionString, this.Settings.SqlBulkCopyOptions);
            bulkCopy.BatchSize = this.Settings.BatchSize;
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BulkCopyTimeout = this.Settings.TimeoutSeconds;
            return bulkCopy;
        }

        #endregion
    }
}