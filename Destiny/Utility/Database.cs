using MySql.Data.MySqlClient;
using System;

// CREDITS: Chronicle
namespace Destiny.Utility
{
    // NOTE: This class is temporary.
    public static class DatabaseExtensions
    {
        public static bool GetBool(this DatabaseQuery query, string field)
        {
            return DatabaseExtensions.GetByte(query, field) == 1 ? true : false;
        }

        public static byte GetByte(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? (byte)0 : (byte)query[field];
        }

        public static short GetShort(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? (short)0 : (short)query[field];
        }

        public static int GetInt(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? 0 : (int)query[field];
        }

        public static long GetLong(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? 0 : (long)query[field];
        }

        public static string GetString(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? string.Empty : (string)query[field];
        }

        public static DateTime GetDateTime(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? DateTime.MinValue : (DateTime)query[field];
        }
    }

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

        public bool Read()
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
                return (int)(long)command.ExecuteScalar();
            }
        }
    }
}
