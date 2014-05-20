namespace EntityFramework.BulkLoad.Test.DataBuilders
{
    using System;
    using System.Collections.Generic;

    using EntityFramework.BulkLoad.Test.Model;

    /// <summary>
    ///     The person builder.
    /// </summary>
    public class SampleEntityBuilder
    {
        #region Static Fields

        /// <summary>
        ///     The random.
        /// </summary>
        private static readonly Random Random = new Random();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create.
        /// </summary>
        /// <returns>
        ///     The <see cref="SampleEntity" />.
        /// </returns>
        public SampleEntity Create()
        {
            return new SampleEntity
            {
                Name = "Person" + Random.Next(),
                DateTime = DateTime.Now,
                DateTimeOffset = DateTime.Now,
                Decimal = (decimal)(Random.NextDouble() * 1000),
                NullableDecimal = (decimal)(Random.NextDouble() * 1000),
                Double = Random.NextDouble(),
                NullableDouble = Random.NextDouble(),
                Float = (float)Random.NextDouble(),
                NullableFloat = (float)Random.NextDouble(),
            };
        }

        /// <summary>
        ///     The create many.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<SampleEntity> CreateMany(int count = 100)
        {
            var items = new List<SampleEntity>();

            for (int i = 0; i < count; i++)
            {
                var item = this.Create();
                items.Add(item);
            }

            return items;
        }

        #endregion
    }
}