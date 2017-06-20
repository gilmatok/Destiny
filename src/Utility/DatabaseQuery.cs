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

}
