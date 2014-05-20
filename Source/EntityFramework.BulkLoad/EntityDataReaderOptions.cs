namespace EntityFramework.BulkLoad
{
    /// <summary>
    ///     The entity data reader options.
    /// </summary>
    public class EntityDataReaderOptions
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityDataReaderOptions" /> class.
        /// </summary>
        /// <param name="exposeNullableTypes">
        ///     The expose nullable types.
        /// </param>
        /// <param name="flattenRelatedObjects">
        ///     The flatten related objects.
        /// </param>
        /// <param name="prefixRelatedObjectColumns">
        ///     The prefix related object columns.
        /// </param>
        /// <param name="recreateForeignKeysForEntityFrameworkEntities">
        ///     The recreate foreign keys for entity framework entities.
        /// </param>
        public EntityDataReaderOptions(
            bool exposeNullableTypes,
            bool flattenRelatedObjects,
            bool prefixRelatedObjectColumns,
            bool recreateForeignKeysForEntityFrameworkEntities)
        {
            this.ExposeNullableTypes = exposeNullableTypes;
            this.FlattenRelatedObjects = flattenRelatedObjects;
            this.PrefixRelatedObjectColumns = prefixRelatedObjectColumns;
            this.RecreateForeignKeysForEntityFrameworkEntities = recreateForeignKeysForEntityFrameworkEntities;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the default.
        /// </summary>
        public static EntityDataReaderOptions Default
        {
            get
            {
                return new EntityDataReaderOptions(true, false, true, false);
            }
        }

        /// <summary>
        ///     If true nullable value types are returned directly by the DataReader.
        ///     If false, the DataReader will expose non-nullable value types and return DbNull.Value
        ///     for null values.
        ///     When loading a DataTable this option must be set to True, since DataTable does not support
        ///     nullable types.
        /// </summary>
        public bool ExposeNullableTypes { get; set; }

        /// <summary>
        ///     If True then the DataReader will project scalar properties from related objects in addition
        ///     to scalar properties from the main object.  This is especially useful for custom projecttions like
        ///     var q = from od in db.SalesOrderDetail
        ///     select new
        ///     {
        ///     od,
        ///     ProductID=od.Product.ProductID,
        ///     ProductName=od.Product.Name
        ///     };
        ///     Related objects assignable to EntityKey, EntityRelation, and IEnumerable are excluded.
        ///     If False, then only scalar properties from teh main object will be projected.
        /// </summary>
        public bool FlattenRelatedObjects { get; set; }

        /// <summary>
        ///     If True columns projected from related objects will have column names prefixed by the
        ///     name of the relating property.  This appies to either from setting FlattenRelatedObjects to True,
        ///     or RecreateForeignKeysForEntityFrameworkEntities to True.
        ///     If False columns will be created for related properties that are not prefixed.  This can lead
        ///     to column name collision.
        /// </summary>
        public bool PrefixRelatedObjectColumns { get; set; }

        /// <summary>
        ///     If True the DataReader will create columns for the key properties of related Entities.
        ///     You must pass an ObjectContext and have retrieved the entity with change tracking for this to work.
        /// </summary>
        public bool RecreateForeignKeysForEntityFrameworkEntities { get; set; }

        #endregion
    }
}