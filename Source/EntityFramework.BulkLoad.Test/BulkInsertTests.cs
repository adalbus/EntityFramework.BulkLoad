namespace EntityFramework.BulkLoad.Test
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using EntityFramework.BulkLoad.Test.Model;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Ploeh.AutoFixture;

    [TestClass]
    public class BulkInsertTests
    {
        #region Static Fields

        private static string connectionString;

        #endregion

        #region Public Properties

        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            connectionString = SqlTestUtils.CreateLocalDbConnectionString(testContext);
            DeploySampleDatabase(connectionString);
        }

        [TestMethod]
        public void BulkInsertTest()
        {
            var fixture = new Fixture();

            IEnumerable<SampleEntity> entities = fixture.CreateMany<SampleEntity>(2);

            var loader = new EntityBulkLoader(connectionString);
            loader.BulkInsert(entities, new SampleContext(connectionString));

            this.VerifyEntitiesWereInserted(entities);
        }

        #endregion

        #region Methods

        private static void DeploySampleDatabase(string destinationConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SampleContext>());

            using (var context = new SampleContext(destinationConnectionString))
            {
                context.SampleEntities.Load();
            }
        }

        private void VerifyEntitiesWereInserted(IEnumerable<SampleEntity> expectedEntities)
        {
            using (var context = new SampleContext(connectionString))
            {
                List<SampleEntity> actualEntities = context.SampleEntities.ToList();

                actualEntities.Should().NotBeEmpty().And.HaveCount(expectedEntities.Count());

                //actualEntities.ShouldAllBeEquivalentTo(expectedEntities, options => options
                //    .Using<SampleEntity>(c => c.Subject.DateTime.Should().BeCloseTo(c.Subject.DateTime, 1000)));
            }
        }

        #endregion
    }
}