namespace EntityFramework.BulkLoad
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataLoader
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Writes the entries to the destination.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items to insert.</param>
        /// <param name="destinationEntitySet">The destination table name.</param>
        void BulkInsert<T>(IEnumerable<T> items, string destinationEntitySet);

        /// <summary>
        ///     Writes the entries to the destination in an asycronous fashion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items to insert.</param>
        /// <param name="destinationEntitySet">The destination table name.</param>
        /// <returns>
        ///     A task that will complete when the bulk operation is completed.
        /// </returns>
        Task BulkInsertAsync<T>(IEnumerable<T> items, string destinationEntitySet);

        #endregion
    }
}