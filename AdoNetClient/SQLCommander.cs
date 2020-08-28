﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetClient
{
    class SQLCommander
    {
        public SQLCommander(IUserInteractor userInteractor)
        {
            this.userInteractor = userInteractor;
        }
        IUserInteractor userInteractor;
        public async Task RunCommandSession(SqlConnection connection, SelectionModes mode)
        {
            if (mode == SelectionModes.Free)
            {
                Console.WriteLine("Enter commant");
                var (sql, dataOutputWay) = userInteractor.ReadCommandInformation();
                await RunCommand(sql, connection, dataOutputWay, null);
            }
            else if (mode == SelectionModes.Predefined)
            {
                var queryInformation = FindQueryInformation();
                await RunCommand(queryInformation.Query, connection, queryInformation.OutputWay, queryInformation.ProcedureInformation);
            }
        }
        private async Task RunCommand(string sql, SqlConnection connection, DataOutputWays dataOutputWay, ProcedureInformation procedureInformation)
        {
            using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
            {
                sqlCommand.CommandTimeout = int.MaxValue;
                await RunCommand(sqlCommand, dataOutputWay, procedureInformation);
            }
        }
        private QueryInformation FindQueryInformation()
        {
            QueryRepository queryRepository = new QueryRepository(userInteractor);
            userInteractor.ShowSuggestions(queryRepository.repository);
            return userInteractor.SelectQuery(queryRepository.repository);
        }
        private async Task RunCommand(SqlCommand sqlCommand, DataOutputWays dataOutputWay, ProcedureInformation procedureInformation)
        {
            switch (dataOutputWay)
            {
                case DataOutputWays.executeNoQuery:
                    await RunExecutionExecuteNonQuery(sqlCommand);
                    break;
                case DataOutputWays.executeReader:
                    await RunExecutionReader(sqlCommand);
                    break;
                case DataOutputWays.executeScalar:
                    await RunExecutionExecuteeScalar(sqlCommand);
                    break;
                case DataOutputWays.executeProcedure:
                    await RunProcedure(sqlCommand, procedureInformation);
                    break;
            }
        }
        private async Task RunExecutionReader(SqlCommand sqlCommand)
        {
            using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                await userInteractor.WriteCommandResult(sqlDataReader);
            }
        }
        private async Task RunExecutionExecuteNonQuery(SqlCommand sqlCommand)
        {
            var rowCount = await sqlCommand.ExecuteNonQueryAsync();
            userInteractor.WriteCountAffectedRows(rowCount);
        }
        private async Task RunExecutionExecuteeScalar(SqlCommand sqlCommand)
        {
            var scalar = await sqlCommand.ExecuteScalarAsync();
            userInteractor.WriteScalar(scalar);
        }
        private async Task RunProcedure(SqlCommand sqlCommand, ProcedureInformation procedureInformation)
        {
            //var par = sqlCommand.CreateParameter();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            if (procedureInformation == null)
            {
                procedureInformation = userInteractor.FillProcedureInformation();
            }
            if (procedureInformation.Parameters != null)
            {
                sqlCommand.Parameters.AddRange(procedureInformation.Parameters);
            }
            switch (procedureInformation.OutputWay)
            {
                case DataOutputWays.executeNoQuery:
                    await RunExecutionExecuteNonQuery(sqlCommand);
                    break;
                case DataOutputWays.executeReader:
                    await RunExecutionReader(sqlCommand);
                    break;
                case DataOutputWays.executeScalar:
                    await RunExecutionExecuteeScalar(sqlCommand);
                    break;
            }
            //SqlParameter par = new SqlParameter("@randomString", "".PadLeft(200));
            //par.Direction = System.Data.ParameterDirection.InputOutput;
            //sqlCommand.Parameters.Add(new SqlParameter("@minLength", Console.ReadLine()));
            //sqlCommand.Parameters.Add(new SqlParameter("@maxLength", Console.ReadLine()));
            //sqlCommand.Parameters.Add(new SqlParameter("@chars", "qwerfdvbhyun"));
            //sqlCommand.Parameters.Add(par);
            //await sqlCommand.ExecuteNonQueryAsync();
            //var s = sqlCommand.Parameters["@randomString"].Value;
            //var ss = sqlCommand.Parameters["@randomString"].SqlValue;
            //var sss = sqlCommand.Parameters["@randomString"].Value.ToString();
            //Console.WriteLine(sqlCommand.Parameters["@randomString"].Value);
            ////using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            ////{
            ////    userInteractor.WriteCommandResult(sqlDataReader);
            ////}[PickRandomStringg]
        }
    }
}