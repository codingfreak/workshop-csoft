namespace Logic.DataAccess.TableStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public interface ITableStorageAdapter<T>
        where T : ITableEntity, new()
    {
        #region methods

        Task<IEnumerable<T>> GetAllAsync();

        #endregion
    }

    public class TableStorageAdapter<T> : ITableStorageAdapter<T>
        where T : ITableEntity, new()
    {
        #region constructors and destructors

        public TableStorageAdapter(TableStorageAdapterSettings settings)
        {
            Settings = settings;
        }

        #endregion

        #region explicit interfaces

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(Settings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(Settings.TableName);
            if (!await table.ExistsAsync())
            {
                return null;
            }
            TableContinuationToken token = null;
            var query = new TableQuery<T>();
            var result = new List<T>();
            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, token, null, null).ConfigureAwait(false);
                result.AddRange(segment.Results);
                token = segment.ContinuationToken;
            }
            while (token != null);
            return result;
        }

        #endregion

        #region properties

        private TableStorageAdapterSettings Settings { get; }

        #endregion
    }
}