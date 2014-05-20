namespace EntityFramework.BulkLoad.Test
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations.History;

    using EntityFramework.BulkLoad.Test.Model;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Ploeh.AutoFixture;

    [TestClass]
    public class BulkInsertTests
    {
        #region Public Properties

        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DeploySampleDatabase(testContext);
        }

        [TestMethod]
        public void BulkInsertTest()
        {
            string connectionString = GetConnectionString();

            var fixture = new Fixture();

            IEnumerable<SampleEntity> entities = fixture.CreateMany<SampleEntity>(2);

            var loader = new EntityBulkLoader(connectionString);
            loader.BulkInsert(entities, "SampleEntity", new SampleContext(connectionString));

            VerifyEntitiesWereInserted(entities);
        }

        #endregion

        #region Methods

        private static void DeploySampleDatabase(TestContext testContext)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SampleContext>());

            string connectionString = SqlTestUtils.CreateConnectionString(testContext);

            using (var context = new SampleContext(connectionString))
            {
                context.SampleEntities.Load();
            }
        }

        private void VerifyEntitiesWereInserted(IEnumerable<SampleEntity> expectedEntities)
        {
            using (var context = new SampleContext(this.GetConnectionString()))
            {
                context.SampleEntities.Should().Contain(expectedEntities);
            }
        }

        private string GetConnectionString()
        {
            return SqlTestUtils.CreateConnectionString(this.TestContext);
        }

        #endregion
    }
}