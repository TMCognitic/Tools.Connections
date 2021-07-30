using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Tools.Connections
{
    public class Connection
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _providerFactory;

        public Connection(string connectionString, DbProviderFactory providerFactory)
        {
            if (providerFactory is null)
                throw new ArgumentException("providerFactory is not valid !");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is not valid !");

            _connectionString = connectionString;
            _providerFactory = providerFactory;

            try
            {
                using (DbConnection dbConnection = CreateConnection())
                {
                    dbConnection.Open();
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("the connection string is not valid or the server is down...");
            }
        }

        public int ExecuteNonQuery(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    return dbCommand.ExecuteNonQuery();
                }                
            }
        }

        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    using (DbDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            yield return selector(dataReader);
                        }
                    }
                }
            }
        }

        public object ExecuteScalar(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    object result = dbCommand.ExecuteScalar();

                    return result is DBNull ? null : result;
                }
            }
        }

        public DataTable GetDataTable(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = _providerFactory.CreateDataAdapter())
                    {
                        DataTable dataTable = new DataTable();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataTable);

                        return dataTable;
                    }
                }
            }
        }

        public DataSet GetDataSet(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = _providerFactory.CreateDataAdapter())
                    {
                        DataSet dataSet = new DataSet();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataSet);

                        return dataSet;
                    }
                }
            }
        }

        private DbConnection CreateConnection()
        {
            DbConnection dbConnection = _providerFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;

            return dbConnection;
        }

        private static DbCommand CreateCommand(Command command, DbConnection dbConnection)
        {
            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            foreach (KeyValuePair<string, object> parameter in command.Parameters)
            {
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;

                dbCommand.Parameters.Add(dbParameter);
            }

            return dbCommand;
        }
    }
}
