using MySql.Data.MySqlClient;
using System;

namespace Destiny.Utility
{
    public sealed class DatabaseQuery : IDisposable
    {
        private MySqlConnection mConnection;
        private MySqlDataReader mReader;

        public object this[string field]
        {
            get
            {
                return mReader[field];
            }
        }

        public DatabaseQuery(MySqlConnection connection, MySqlDataReader reader)
        {
            mConnection = connection;
            mReader = reader;
        }

        public bool NextRow()
        {
            return mReader.Read();
        }

        public void Dispose()
        {
            mReader.Close();
            mConnection.Close();
        }
    }

    public sealed class Database
    {
        private static string sConnectionString;

        public static void Initialize()
        {
            sConnectionString = string.Format("Server={0}; Database={1}; Username={2}; Password={3}; Pooling=true; Min Pool Size=4; Max Pool Size=32",
                               Config.Instance.Database.Host,
                               Config.Instance.Database.Schema,
                               Config.Instance.Database.Username,
                               Config.Instance.Database.Password);

            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();

                Logger.Write(LogLevel.Info, "Able to connect to database '{0}'.", Config.Instance.Database.Schema);

                connection.Close();
            }
        }

        public static DatabaseQuery Query(string query, params MySqlParameter[] args)
        {
            MySqlConnection connection = new MySqlConnection(sConnectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            Array.ForEach(args, p => command.Parameters.Add(p));
            return new DatabaseQuery(connection, command.ExecuteReader());
        }

        public static void Execute(string query, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                Array.ForEach(args, p => command.Parameters.Add(p));
                command.ExecuteNonQuery();
            }
        }

        public static object Scalar(string query, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                Array.ForEach(args, p => command.Parameters.Add(p));
                return command.ExecuteScalar();
            }
        }

        public static int InsertAndReturnIdentifier(string query, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                Array.ForEach(args, p => command.Parameters.Add(p));
                command.ExecuteNonQuery();
                command.CommandText = "SELECT LAST_INSERT_ID()";
                command.Parameters.Clear();
                return (int)(ulong)command.ExecuteScalar();
            }
        }
    }
}
