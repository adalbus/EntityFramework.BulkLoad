namespace EntityFramework.BulkLoad
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     The entity data reader extensions.
    /// </summary>
    public static class EntityDataReaderExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Wraps the IEnumerable in a DbDataReader, having one column for each "scalar" property of the type T.
        ///     The collection will be enumerated as the client calls IDataReader.Read().
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="collection">
        /// </param>
        /// <returns>
        ///     The <see cref="IDataReader" />.
        /// </returns>
        public static IDataReader AsDataReader<T>(this IEnumerable<T> collection)
        {
            // For anonymous type projections default to flattening related objects and not prefixing columns
            // The reason being that if the programmer has taken control of the projection, the default should
            // be to expose everying in the projection and not mess with the names.
            if (typeof(T).IsDefined(typeof(CompilerGeneratedAttribute), false))
            {
                EntityDataReaderOptions options = EntityDataReaderOptions.Default;
                options.FlattenRelatedObjects = true;
                options.PrefixRelatedObjectColumns = false;
                return new EntityDataReader<T>(collection, (ObjectContext)null);
            }

            return new EntityDataReader<T>(collection, (ObjectContext)null);
        }

        /// <summary>
        ///     Wraps the IEnumerable in a DbDataReader, having one column for each "scalar" property of the type T.
        ///     The collection will be enumerated as the client calls IDataReader.Read().
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="collection">
        /// </param>
        /// <param name="exposeNullableColumns">
        ///     The expose Nullable Columns.
        /// </param>
        /// <param name="flattenRelatedObjects">
        ///     The flatten Related Objects.
        /// </param>
        /// <returns>
        ///     The <see cref="IDataReader" />.
        /// </returns>
        public static IDataReader AsDataReader<T>(
            this IEnumerable<T> collection,
            bool exposeNullableColumns,
            bool flattenRelatedObjects)
        {
            var options = new EntityDataReaderOptions(exposeNullableColumns, flattenRelatedObjects, true, false);

            return new EntityDataReader<T>(collection, (ObjectContext)null);
        }

        /// <summary>
        ///     Wraps the collection in a DataReader, but also includes columns for the key attributes of related Entities.
        /// </summary>
        /// <typeparam name="T">
        ///     The element type of the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     A collection to wrap in a DataReader
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     A DbDataReader wrapping the collection.
        /// </returns>
        public static IDataReader AsDataReader<T>(this IEnumerable<T> collection, ObjectContext context)
        {
            EntityDataReaderOptions options = EntityDataReaderOptions.Default;
            options.RecreateForeignKeysForEntityFrameworkEntities = true;
            return new EntityDataReader<T>(collection, context);
        }

        public static IDataReader AsDataReader<T>(this IEnumerable<T> collection, DbContext context)
        {
            EntityDataReaderOptions options = EntityDataReaderOptions.Default;
            options.RecreateForeignKeysForEntityFrameworkEntities = true;
            return new EntityDataReader<T>(collection, context);
        }

        /// <summary>
        ///     The as data reader.
        /// </summary>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="detachObjects">
        ///     The detach objects.
        /// </param>
        /// <param name="prefixRelatedObjectColumns">
        ///     The prefix related object columns.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IDataReader" />.
        /// </returns>
        public static IDataReader AsDataReader<T>(
            this IEnumerable<T> collection,
            ObjectContext context,
            bool detachObjects,
            bool prefixRelatedObjectColumns)
        {
            EntityDataReaderOptions options = EntityDataReaderOptions.Default;
            options.RecreateForeignKeysForEntityFrameworkEntities = true;
            options.PrefixRelatedObjectColumns = prefixRelatedObjectColumns;

            if (detachObjects)
            {
                return new EntityDataReader<T>(collection.DetachAllFrom(context), context);
            }

            return new EntityDataReader<T>(collection, context);
        }

        /// <summary>
        ///     Enumerates the collection and copies the data into a DataTable.
        /// </summary>
        /// <typeparam name="T">
        ///     The element type of the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to copy to a DataTable
        /// </param>
        /// <returns>
        ///     A DataTable containing the scalar projection of the collection.
        /// </returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var t = new DataTable();
            t.Locale = CultureInfo.CurrentCulture;
            t.TableName = typeof(T).Name;
            EntityDataReaderOptions options = EntityDataReaderOptions.Default;
            options.ExposeNullableTypes = false;
            var dr = new EntityDataReader<T>(collection, (ObjectContext)null);
            t.Load(dr);
            return t;
        }

        /// <summary>
        ///     Enumerates the collection and copies the data into a DataTable, but also includes columns for the key attributes of
        ///     related Entities.
        /// </summary>
        /// <typeparam name="T">
        ///     The element type of the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to copy to a DataTable
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     A DataTable containing the scalar projection of the collection.
        /// </returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, ObjectContext context)
            where T : EntityObject
        {
            var t = new DataTable();
            t.Locale = CultureInfo.CurrentCulture;
            t.TableName = typeof(T).Name;

            EntityDataReaderOptions options = EntityDataReaderOptions.Default;
            options.RecreateForeignKeysForEntityFrameworkEntities = true;

            var dr = new EntityDataReader<T>(collection, context);
            t.Load(dr);
            return t;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The detach all from.
        /// </summary>
        /// <param name="col">
        ///     The col.
        /// </param>
        /// <param name="context">
        ///     The cx.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        private static IEnumerable<T> DetachAllFrom<T>(this IEnumerable<T> col, ObjectContext context)
        {
            foreach (T t in col)
            {
                context.Detach(t);
                yield return t;
            }
        }

        #endregion
    }
}