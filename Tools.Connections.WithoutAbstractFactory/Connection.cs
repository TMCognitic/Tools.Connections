using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Tools.Connections.WithoutAbstractFactory
{
    public class Connection
    {
        private readonly string _connectionString;

        public Connection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is not valid !");

            _connectionString = connectionString;

            try
            {
                using(SqlConnection dbConnection = CreateConnection())
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
            using (SqlConnection dbConnection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    return dbCommand.ExecuteNonQuery();
                }                
            }
        }

        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            using (SqlConnection dbConnection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    using (SqlDataReader dataReader = dbCommand.ExecuteReader())
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
            using (SqlConnection dbConnection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    object result = dbCommand.ExecuteScalar();

                    return result is DBNull ? null : result;
                }
            }
        }

        public DataTable GetDataTable(Command command)
        {
            using (SqlConnection dbConnection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (SqlDataAdapter dbDataAdapter = new SqlDataAdapter())
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
            using (SqlConnection dbConnection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (SqlDataAdapter dbDataAdapter = new SqlDataAdapter())
                    {
                        DataSet dataSet = new DataSet();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataSet);

                        return dataSet;
                    }
                }
            }
        }

        private SqlConnection CreateConnection()
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = _connectionString;

            return dbConnection;
        }

        private static SqlCommand CreateCommand(Command command, SqlConnection dbConnection)
        {
            SqlCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            foreach (KeyValuePair<string, object> parameter in command.Parameters)
            {
                SqlParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;

                dbCommand.Parameters.Add(dbParameter);
            }

            return dbCommand;
        }
    }
}
