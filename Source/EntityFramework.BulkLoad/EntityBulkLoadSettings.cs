namespace EntityFramework.BulkLoad
{
    using System.Data.SqlClient;

    public class EntityBulkLoadSettings
    {
        #region Constants

        /// <summary>
        ///     The default batch size
        /// </summary>
        public const int DefaultBatchSize = 10000;

        /// <summary>
        ///     The default timeout seconds
        /// </summary>
        public const int DefaultTimeoutSeconds = 2 * 60 * 60;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityBulkLoadSettings" /> class.
        /// </summary>
        public EntityBulkLoadSettings()
        {
            this.BatchSize = DefaultBatchSize;
            this.TimeoutSeconds = DefaultTimeoutSeconds;
            this.SqlBulkCopyOptions = SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the batch size.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        ///     Gets or sets the sql bulk copy options.
        /// </summary>
        public SqlBulkCopyOptions SqlBulkCopyOptions { get; set; }

        /// <summary>
        ///     Gets or sets the timeout seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; }

        #endregion
    }
}