using MySql.Data.MySqlClient;
using System;

namespace Destiny.Utility
{
    public sealed class Database
    {
        public static string Host { get; set; }
        public static string Schema { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }

        public static string ConnectionString
        {
            get
            {
                return string.Format("Server={0}; Database={1}; Username={2}; Password={3}; Pooling=true; Min Pool Size=4; Max Pool Size=32",
                                     Database.Host,
                                     Database.Schema,
                                     Database.Username,
                                     Database.Password);
            }
        }

        public static void Initialize()
        {
            Database.Host = "localhost";
            Database.Schema = "destiny";
            Database.Username = "root";
            Database.Password = "root";

            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();

                Logger.Write(LogLevel.Info, "Able to connect to database '{0}'.", "destiny");

                connection.Close();
            }
        }

        public static TemporarySchema TemporarySchema(string schema)
        {
            return new TemporarySchema(schema);
        }

        public static DatabaseQuery Query(string query, params MySqlParameter[] args)
        {
            MySqlConnection connection = new MySqlConnection(Database.ConnectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            Array.ForEach(args, p => command.Parameters.Add(p));
            return new DatabaseQuery(connection, command.ExecuteReader());
        }

        public static void Execute(string query, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
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

    public sealed class TemporarySchema : IDisposable
    {
        public TemporarySchema(string schema)
        {
            Database.Schema = schema;
        }

        public void Dispose()
        {
            Database.Schema = "Destiny";
        }
    }
}
