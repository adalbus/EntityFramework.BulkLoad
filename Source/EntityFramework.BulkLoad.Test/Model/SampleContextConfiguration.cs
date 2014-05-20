namespace EntityFramework.BulkLoad.Test.Model
{
    using System.Data.Entity;
    using System.Data.Entity.SqlServer;

    public class SampleContextConfiguration : DbConfiguration
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SampleContextConfiguration" /> class.
        /// </summary>
        public SampleContextConfiguration()
        {
            this.SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }

        #endregion
    }
}