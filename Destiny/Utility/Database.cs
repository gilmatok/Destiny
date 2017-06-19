using MySql.Data.MySqlClient;
using System;
using System.IO;

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

        public static void Test()
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();

                Log.Inform("Able to connect to database '{0}'.", Database.Schema);

                connection.Close();
            }
        }

        public static TemporarySchema TemporarySchema(string schema)
        {
            return new TemporarySchema(schema);
        }

        public static TemporaryConnection TemporaryConnection(string host, string schema, string username, string password)
        {
            return new TemporaryConnection(host, schema, username, password);
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

        public static void ExecuteScript(string host, string username, string password, string query, params object[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(string.Format("SERVER={0}; UID={1}; PASSWORD={2};", host, username, password)))
            {
                connection.Open();
                new MySqlScript(connection, string.Format(query, args)).Execute();
                connection.Close();
            }
        }

        public static void ExecuteFile(string host, string username, string password, string path)
        {
            using (MySqlConnection connection = new MySqlConnection(string.Format("SERVER={0}; UID={1}; PASSWORD={2};", host, username, password)))
            {
                connection.Open();

                using (TextReader reader = new StreamReader(path))
                {
                    new MySqlScript(connection, reader.ReadToEnd()).Execute();
                }

                connection.Close();
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

    public class TemporarySchema : IDisposable
    {
        private string oldSchema;

        internal TemporarySchema(string schema)
        {
            this.oldSchema = Database.Schema;
            Database.Schema = schema;
        }

        public void Dispose()
        {
            Database.Schema = this.oldSchema;
        }
    }

    public class TemporaryConnection : IDisposable
    {
        private string oldHost;
        private string oldSchema;
        private string oldUsername;
        private string oldPassword;

        internal TemporaryConnection(string host, string schema, string username, string password)
        {
            this.oldHost = Database.Host;
            this.oldSchema = Database.Schema;
            this.oldUsername = Database.Username;
            this.oldPassword = Database.Password;

            Database.Host = host;
            Database.Schema = schema;
            Database.Username = username;
            Database.Password = password;
        }

        public void Dispose()
        {
            Database.Host = this.oldHost;
            Database.Schema = this.oldSchema;
            Database.Username = this.oldUsername;
            Database.Password = this.oldPassword;
        }
    }
}
