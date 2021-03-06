﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdoNetClient
{
    interface IUserInteractor
    {
        //void Write(string value);
        //void Write(char value);
        //void WriteLine(string value);
        //void WriteLine();
        SelectionMode SelectMode();
        CommandPrototype ReadCommandInformation();
        Task WriteCommandResult(SqlDataReader sqlDataReader, CancellationToken cancellationTokenSource);
        void WriteCountAffectedRows(int rowsCount);
        void WriteScalar(object value);
        SqlParameter[] ReadSqlParameters();
        void ShowSuggestions(Dictionary<string, QueryInformation> repository);
        QueryInformation SelectQuery(Dictionary<string, QueryInformation> repository);
        string ReadParameter(string message);
        void WriteExceptionMessage(Exception ex);
        void WriteParametersResult(SqlParameterCollection sqlParameterCollection);
        Task SelectContinuation(CancellationTokenSource cancellationTokenSource, Task task);
    }
}
