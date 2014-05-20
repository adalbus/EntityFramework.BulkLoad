namespace EntityFramework.BulkLoad.Test.Model
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class SampleContext : DbContext
    {
        #region Constructors and Destructors

        public SampleContext()
        {
        }

        public SampleContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        #endregion

        #region Public Properties

        public IDbSet<SampleEntity> SampleEntities { get; set; }

        #endregion

        #region Methods

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        #endregion
    }
}