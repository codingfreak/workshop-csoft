using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.DataAccess.TableStorage
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class TelemeryTableEntity : TableEntity
    {
        public long Temperature { get; set; }
    }

}
