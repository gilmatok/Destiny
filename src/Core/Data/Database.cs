using Destiny.Core.IO;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Linq;

namespace Destiny.Core.Data
{
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

    public static class Database
    {
        public static string Host { get; set; }
        public static string Schema { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }

        internal static string CorrectFields(string fields)
        {
            string final = string.Empty;
            string[] tokens = fields.Replace(",", " ").Replace(";", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int processed = 0;

            foreach (string field in tokens)
            {
                final += "`" + field + "`";
                processed++;

                if (processed < tokens.Length)
                {
                    final += ", ";
                }
            }

            return final;
        }

        internal static string ConnectionString
        {
            get
            {
                return string.Format("server={0}; database={1}; uid={2}; password={3}; convertzerodatetime=yes;",
                    Database.Host,
                    Database.Schema,
                    Database.Username,
                    Database.Password);
            }
        }

        internal static void Execute(string nonQuery, params object[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = GetCommand(connection, nonQuery, args))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        internal static MySqlDataReader ExecuteReader(string query, params object[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = GetCommand(connection, query, args))
                {
                    return command.ExecuteReader();
                }
            }
        }

        public static object Scalar(string nonQuery, params object[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = GetCommand(connection, nonQuery, args))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        public static string DefaultSchema
        {
            get
            {
                return Settings.GetString("Database/Schema");
            }
        }

        public static void Test()
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();
                Log.Inform("Able to connect to database '{0}'.", connection.Database);
                connection.Close();
            }
        }

        public static void Analyze(bool mcdb)
        {
            using (Database.TemporarySchema("information_schema"))
            {
                Meta.Initialize(mcdb);
            }
        }

        public static void Delete(string table, string constraints, params object[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = GetCommand(connection, constraints, args))
                {
                    command.CommandText = string.Format("DELETE FROM `{0}` WHERE ", table) + command.CommandText;

                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool Exists(string table, string constraints, params object[] args)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = GetCommand(connection, constraints, args))
                {
                    command.CommandText = string.Format("SELECT * FROM `{0}` WHERE ", table) + command.CommandText;

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static dynamic Fetch(string table, string field, string constraints, params object[] args)
        {
            object value = new Datum(table).PopulateWith(field, constraints, args).Dictionary[field];

            if (value is DBNull)
            {
                return null;
            }
            else if (value is byte && Meta.IsBool(table, field))
            {
                return (byte)value > 0;
            }
            else
            {
                return value;
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

        public static TemporaryConnection TemporaryConnection(string host, string schema, string username, string password)
        {
            return new TemporaryConnection(host, schema, username, password);
        }

        public static TemporarySchema TemporarySchema(string schema)
        {
            return new TemporarySchema(schema);
        }

        public static MySqlCommand GetCommand(MySqlConnection connection, string str, params object[] args)
        {
            MySqlCommand command = new MySqlCommand(ParameterizeCommandText("param", str, args), connection);

            command.Parameters.AddRange(ConstraintsToParameters("param", str, args));

            return command;
        }

        public static string ParameterizeCommandText(string namePrefix, string commandText, params object[] args)
        {
            return commandText != null ? string.Format(commandText, args?.Select((v, i) => "@" + namePrefix + i).ToArray()) : string.Empty;
        }

        public static MySqlParameter[] ConstraintsToParameters(string namePrefix, string constraints, params object[] args)
        {
            MySqlParameter[] parameters = new MySqlParameter[args != null ? args.Length : 0];

            for (int i = 0; i < args?.Length; i++)
            {
                parameters[i] = new MySqlParameter("@" + namePrefix + i, args[i]);
            }

            return parameters;
        }
    }
}
