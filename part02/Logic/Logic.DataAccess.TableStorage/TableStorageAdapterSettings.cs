using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.DataAccess.TableStorage
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    public class TableStorageAdapterSettings
    {

        public TableStorageAdapterSettings(IConfiguration config)
        {
            ConnectionString = config["Storage:ConnectionString"];
            TableName = config["Storage:TableName"];
        }
        public string ConnectionString { get; }

        public string TableName { get; }

    }
}
