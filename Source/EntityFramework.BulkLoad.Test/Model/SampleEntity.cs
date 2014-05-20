namespace EntityFramework.BulkLoad.Test.Model
{
    using System;

    public class SampleEntity
    {
        #region Public Properties

        public DateTime DateTime { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public decimal Decimal { get; set; }

        public double Double { get; set; }

        public float Float { get; set; }

        public Guid Guid { get; set; }

        public int Id { get; set; }

        public long Long { get; set; }

        public string Name { get; set; }

        public Guid? Nullable { get; set; }

        public decimal? NullableDecimal { get; set; }

        public double? NullableDouble { get; set; }

        public float? NullableFloat { get; set; }

        public int? NullableInteger { get; set; }

        public long? NullableLong { get; set; }

        #endregion
    }
}