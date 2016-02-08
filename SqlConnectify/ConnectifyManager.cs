using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlConnectify
{
    interface IConnectifyManager
    {
        bool TestConnection();
        Task<bool> TestConnectionAsync();

        DataTable Read(string query);
        Task<DataTable> ReadAsync(string query);
    }

    public class ConnectifyManager : IConnectifyManager
    {
        private string _connectionString;

        public ConnectifyManager()
        {
            _connectionString = ConfigurationManager.AppSettings["ConnectifyConnectionString"];
        }

        public ConnectifyManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ConfigConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        private void Log(string format, params object[] args)
        {
            Logify.Log(format, args);
        }

        public bool TestConnection()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    Log("test connection with cs: {0} has error: {1}", _connectionString, ex);
                    return false;
                }
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    return connection.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    Log("test connection with cs: {0} has error: {1}", _connectionString, ex);
                    return false;
                }
            }
        }

        public DataTable Read(string query)
        {
            var passed = TestConnection();

            if (!passed)
            {                
                return null;
            }

            DataTable table = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CommandBuilder(connection, query);

                using (var adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        var rows = adapter.Fill(table);
                        Log("query {0} has {1} results", query, rows);
                    }
                    catch (Exception ex)
                    {
                        Log("fill table by query {0} error: {1}", query, ex.Message);
                    }
                }
            }
            
            return table;
        }

        public async Task<DataTable> ReadAsync(string query)
        {
            var passed = await TestConnectionAsync();

            if (!passed)
            {
                Log("test connection with cs: {0} fail!", _connectionString);
                return null;
            }

            DataTable table = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CommandBuilder(connection, query);

                using (var adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        var rows = adapter.Fill(table);
                        Log("query {0} has {1} results", query, rows);
                    }
                    catch (Exception ex)
                    {
                        Log("fill table by query {0} error: {1}", query, ex.Message);
                    }
                }
            }

            return table;
        }

        public DataTable ReadUsingParameter(string query, Hashtable parameters)
        {
            var passed = TestConnection();
            if (!passed)
            {
                return null;
            }

            DataTable table = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CommandBuilder(connection, query);
                var commandParameters = ParameterBuilder(parameters);
                command.Parameters.AddRange(commandParameters);
                
                using (var adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        var rows = adapter.Fill(table);
                        Log("query {0} has {1} results", query, rows);
                    }
                    catch (Exception ex)
                    {
                        Log("fill table by query {0} error: {1}", query, ex.Message);
                    }
                }
            }

            return table;
        }

        public async Task<DataTable> ReadUsingParameterAsync(string query, Hashtable parameters)
        {
            var passed = await TestConnectionAsync();
            if (!passed)
            {
                return null;
            }

            DataTable table = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CommandBuilder(connection, query);
                var commandParameters = ParameterBuilder(parameters);
                command.Parameters.AddRange(commandParameters);

                using (var adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        var rows = adapter.Fill(table);
                        Log("query {0} has {1} results", query, rows);
                    }
                    catch (Exception ex)
                    {
                        Log("fill table by query {0} error: {1}", query, ex.Message);
                    }
                }
            }

            return table;
        }

        private SqlParameter[] ParameterBuilder(Hashtable parameters)
        {
            return null;
        }

        private SqlCommand CommandBuilder(SqlConnection connection, string query)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandTypeSelector(query);
            // var res = command.BeginExecuteNonQuery();
            // command.EndExecuteNonQuery(res);

            return command; 
        }

        private CommandType CommandTypeSelector(string query)
        {
            var type = CommandType.StoredProcedure;
            if (query.Contains(" ")) type = CommandType.Text;

            return type;
        }
    }
}
