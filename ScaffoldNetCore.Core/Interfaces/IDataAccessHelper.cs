using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldNetCore.Core.Interfaces
{
    public interface IDataAccessHelper
    {
        Task<int> RunAsync();
        Task<int> RunAsync(DataTable dataTable);
        Task<int> RunAsync(DataSet dataSet);
        void CommandWithoutParams(string sprocName);
        void CommandWithParams(string sprocName, SqlParameter[] parameters);
        void Dispose();
    }
}
