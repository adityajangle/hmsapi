
using System;
using System.Data;
using System.Reflection;
using hmsapi.Services;
using MySql.Data.MySqlClient;

namespace hmsapi.Data
{
    public interface IDbOperations
    {
        public void ExecuteTransaction(Action<MySqlConnection, MySqlTransaction, MySqlCommand> transactionAction, bool handleException);
        public void ExecuteNonTransaction(Action<IDbCommand> CommandAction);
        public void ExecuteNonTransaction(Action<MySqlCommand> CommandAction, string query);
        public T? SelectParsedData<T>(T target, DataRow dtb);
        public DataTable ExecuteTable(String commandText, IDictionary<string, object>? dict);
        public int ExecuteUpdate(String commandText, IDictionary<string, object>? dict);
        public void AddCommandParams<T>(T cls, MySqlCommand command);
        public void AddCmdParams(Dictionary<string, object?> cls, MySqlCommand command);
        public string GetDatabaseName();
    }

    public class DbOperations : IDbOperations
    {
        private readonly IConfiguration _configuration;

        public DbOperations(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetDatabaseName()
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("MySQL")))
            {
                return connection.Database;
            }
        }

        public void ExecuteTransaction(Action<MySqlConnection, MySqlTransaction, MySqlCommand> transactionAction, bool handleException = false)
        {

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("MySQL")))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        if (handleException)
                        {
                            try
                            {
                                // Execute the custom action with the connection and transaction
                                transactionAction.Invoke(connection, transaction, command);

                                // Commit the transaction if everything is successful
                                transaction.Commit();

                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction in case of an exception
                                Console.WriteLine($"Transaction failed: {ex.Message}");
                                transaction.Rollback();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                        else
                        {
                            transactionAction.Invoke(connection, transaction, command);

                        }
                    }
                };
            };


        }

        public int ExecuteUpdate(String commandText, IDictionary<string, object>? dict)
        {

            //without filter
            //Global_par g_par = new Global_par();
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("MySQL")))
            {
                connection.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(commandText, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (dict != null)
                        {
                            foreach (var kv in dict)
                            {
                                cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value);
                            }
                        }

                        return cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

        }


        public DataTable ExecuteTable(String commandText, IDictionary<string, object>? dict)
        {

            //without filter
            //Global_par g_par = new Global_par();
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("MySQL")))
            {
                connection.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(commandText, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (dict != null)
                        {
                            foreach (var kv in dict)
                            {
                                cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value);
                            }
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(reader);
                            reader.Close();
                            if (resultTable == null)
                            {
                                throw new Exception("DBNULL");
                            }
                            return resultTable;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

        }



        public void ExecuteNonTransaction(Action<IDbCommand> CommandAction)
        {

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("MySQL")))
            {
                try
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        // Execute the custom action with the connection and transaction
                        CommandAction.Invoke(command);

                    }
                }
                finally
                {
                    connection.Close();
                }

            }

        }

        public void ExecuteNonTransaction(Action<MySqlCommand> CommandAction, string query)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("MySQL")))
            {
                connection.Open();
                try
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        CommandAction.Invoke(command);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public T? SelectParsedData<T>(T target, DataRow dtb)
        {
            if (target == null)
            {
                return target;
            }
            PropertyInfo[] _props = target!.GetType().GetProperties();
            for (int i = 0; i < _props.Length; i++) //(var x in _props)
            {

                if (dtb.Table.Columns.Contains(_props[i].Name))
                {

                    object _val = dtb[_props[i].Name];
                    if (_val is DateTime && _props[i].PropertyType == typeof(DateOnly))
                    {
                        _props[i].SetValue(target, DateOnly.FromDateTime((DateTime)_val));
                    }
                    else if (_val is TimeSpan && _props[i].PropertyType == typeof(TimeOnly))
                    {
                        _props[i].SetValue(target, TimeOnly.FromTimeSpan((TimeSpan)_val));
                    }
                    else if (_val is DBNull)
                    {
                        _props[i].SetValue(target, null);
                    }

                    else if (_val is decimal && _props[i].PropertyType == typeof(double))
                    {
                        _props[i].SetValue(target, Convert.ToDouble(_val));
                    }
                    else if (_val is int && _props[i].PropertyType == typeof(string))
                    {
                        _props[i].SetValue(target, $"{_val}");
                    }
                    else
                    {
                        _props[i].SetValue(target, _val);
                    }
                }

            }

            return target;
        }

        public void AddCmdParams(Dictionary<string, object?> cls, MySqlCommand command)
        {
            foreach (var x in cls)
            {
                if (x.Value == null)
                {
                    command.Parameters.AddWithValue($"@{x.Key}", x.Value);
                }
                else if (x.Value.GetType() == typeof(DateOnly))
                {
                    command.Parameters.AddWithValue($"@{x.Key}", UtilService.DoMSql((DateOnly)x.Value));
                }
                else if (x.Value.GetType() == typeof(DateTime))
                {
                    command.Parameters.AddWithValue($"@{x.Key}", UtilService.DttMSql((DateTime)x.Value));
                }
                else if (x.Value.GetType() == typeof(bool?) || x.Value.GetType() == typeof(bool))
                {
                    command.Parameters.AddWithValue($"@{x.Key}", ((bool)x.Value) ? 1 : 0);
                }
                else if (x.Value.GetType() == typeof(byte[]) || x.Value.GetType() == typeof(byte?[]))
                {
                    command.Parameters.Add($"@{x.Key}", MySqlDbType.Blob).Value = x.Value; //command.Parameters.Add("@blobData", MySqlDbType.Blob).Value = blobData;
                }
                else
                {
                    command.Parameters.AddWithValue($"@{x.Key}", x.Value);
                }
            }
        }

        public void AddCommandParams<T>(T cls, MySqlCommand command)
        {
            if (cls == null)
            {
                throw new ArgumentNullException("");
            }
            cls.GetType().GetProperties().ToList().ForEach(x =>
            {
                if (x.PropertyType == typeof(DateOnly))
                {
                    command.Parameters.AddWithValue($"@{x.Name}", UtilService.DoMSql((DateOnly)x.GetValue(cls)!));
                }
                else if (x.PropertyType == typeof(DateTime))
                {
                    command.Parameters.AddWithValue($"@{x.Name}", UtilService.DttMSql((DateTime)x.GetValue(cls)!));
                }
                else if (x.PropertyType == typeof(bool?) || x.PropertyType == typeof(bool))
                {
                    command.Parameters.AddWithValue($"@{x.Name}", ((bool)x.GetValue(cls)!) ? 1 : 0);
                }
                else if (x.PropertyType == typeof(byte[]) || x.PropertyType == typeof(byte?[]))
                {
                    command.Parameters.Add($"@{x.Name}", MySqlDbType.Blob).Value = x.GetValue(cls); //command.Parameters.Add("@blobData", MySqlDbType.Blob).Value = blobData;
                }
                else
                {
                    command.Parameters.AddWithValue($"@{x.Name}", x.GetValue(cls));
                }
            });
        }

    }
}


