namespace EntityFramework.BulkLoad.Test
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     The SQL test utilities.
    /// </summary>
    public static class SqlTestUtils
    {
        #region Constants

        /// <summary>
        ///     The database default name format.
        /// </summary>
        public const string DatabaseDefaultNameFormat = "{0:yyyyMMdd-hhmmss}-{1}";

        /// <summary>
        ///     The local server name.
        /// </summary>
        public const string LocalhostServerName = "localhost";

        /// <summary>
        ///     The server based connection string format.
        /// </summary>
        public const string SqlServerConnectionStringFormat =
            @"Data Source={0};Initial Catalog={1};Integrated Security=True";

        /// <summary>
        ///     The file based connection string format.
        /// </summary>
        private const string LocalDbConnectionStringFormat =
            @"Server=(localdb)\v11.0;Integrated Security=true;AttachDbFileName=""{0}"";";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the connection string. By default it uses (local).
        /// </summary>
        /// <param name="databaseName">
        ///     Name of the database.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CreateConnectionString(string databaseName)
        {
            return CreateConnectionString(databaseName, LocalhostServerName);
        }

        /// <summary>
        ///     The create connection string.
        /// </summary>
        /// <param name="databaseName">
        ///     The database name.
        /// </param>
        /// <param name="serverName">
        ///     The server name.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <remarks>
        ///     LocalDb is not supported. Only Enterprise edition of SQL Server supports partitioning.
        /// </remarks>
        public static string CreateConnectionString(string databaseName, string serverName)
        {
            return string.Format(SqlServerConnectionStringFormat, serverName, databaseName);
        }

        /// <summary>
        ///     The create connection string.
        /// </summary>
        /// <param name="testContext">
        ///     The test context.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CreateConnectionString(TestContext testContext)
        {
            string databaseName = CreateDefaultDatabaseName(testContext);
            return CreateConnectionString(databaseName);
        }

        public static string CreateLocalDbConnectionString(string databaseName, string databaseDirectory)
        {
            string fileName = string.Format("{0}.mdf", databaseName);
            string databaseFullPath = Path.Combine(databaseDirectory, fileName);

            string connectionString = string.Format(LocalDbConnectionStringFormat, databaseFullPath);
            return connectionString;
        }

        /// <summary>
        ///     Creates a connection string that points to a file database (localDb).
        /// </summary>
        /// <param name="testContext">
        ///     The test context.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CreateLocalDbConnectionString(TestContext testContext)
        {
            string databaseName = CreateDefaultDatabaseName(testContext);
            return CreateLocalDbConnectionString(databaseName, testContext.TestDeploymentDir);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The create default database name.
        /// </summary>
        /// <param name="testContext">
        ///     The test context.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string CreateDefaultDatabaseName(TestContext testContext)
        {
            return string.Format(DatabaseDefaultNameFormat, DateTime.UtcNow, testContext.TestName);
        }

        #endregion
    }
}