using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Destiny.Data
{
    public sealed class Datums : IEnumerable<Datum>
    {
        private string Table { get; set; }
        private List<Datum> Values { get; set; }

        public Datums(string table)
        {
            this.Table = table;
        }

        private void PopulateInternal(string fields, string constraints)
        {
            this.Values = new List<Datum>();

            string query = string.Format("SELECT {0} FROM {1}{2}", fields == null ? "*" : Database.CorrectFields(fields), this.Table, constraints != null ? " WHERE " + constraints : string.Empty);

            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Database.ConnectionString, query))
            {
                while (reader.Read())
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dictionary.Add(reader.GetName(i), reader.GetValue(i));
                    }

                    this.Values.Add(new Datum(this.Table, dictionary));
                }
            }
        }

        public Datums Populate()
        {
            this.PopulateInternal(null, null);

            return this;
        }

        public Datums Populate(string constraints, params object[] args)
        {
            this.PopulateInternal(null, string.Format(constraints, args));

            return this;
        }

        public Datums PopulateWith(string fields)
        {
            this.PopulateInternal(fields, null);

            return this;
        }

        public Datums PopulateWith(string fields, string constraints, params object[] args)
        {
            this.PopulateInternal(fields, string.Format(constraints, args));

            return this;
        }

        public IEnumerator<Datum> GetEnumerator()
        {
            foreach (Datum loopDatum in this.Values)
            {
                yield return loopDatum;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }

    public sealed class Datum
    {
        public string Table { get; private set; }
        public Dictionary<string, Object> Dictionary { get; set; }

        public object this[string name]
        {
            get
            {
                if (this.Dictionary[name] is DBNull)
                {
                    return null;
                }
                else if (this.Dictionary[name] is byte && Meta.IsBool(this.Table, name))
                {
                    return (byte)this.Dictionary[name] > 0;
                }

                return this.Dictionary[name];
            }
            set
            {
                if (value is DateTime)
                {
                    if (Meta.IsDate(this.Table, name))
                    {
                        value = ((DateTime)value).ToString("yyyy-MM-dd");
                    }
                    else if (Meta.IsDateTime(this.Table, name))
                    {
                        value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                this.Dictionary[name] = value;
            }
        }

        public Datum(string table)
        {
            this.Table = table;
            this.Dictionary = new Dictionary<string, object>();
        }

        public Datum(string table, Dictionary<string, object> dictionary)
        {
            this.Table = table;
            this.Dictionary = dictionary;
        }

        public void Populate(string query)
        {
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Database.ConnectionString, query))
            {
                if (reader.RecordsAffected > 1)
                {
                    throw new RowNotUniqueException();
                }

                if (!reader.HasRows)
                {
                    throw new RowNotInTableException();
                }

                reader.Read();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    object value = reader.GetValue(i);

                    this.Dictionary[name] = value;
                }
            }
        }

        public Datum Populate(string constraints, params object[] args)
        {
            this.Populate(string.Format("SELECT * FROM {0} WHERE {1}", this.Table, string.Format(constraints, args)));

            return this;
        }

        public Datum PopulateWith(string fields, string constraints, params object[] args)
        {
            this.Populate(string.Format("SELECT {0} FROM {1} WHERE {2}", Database.CorrectFields(fields), this.Table, string.Format(constraints, args)));

            return this;
        }

        public void Insert()
        {
            string fields = "( ";

            int processed = 0;

            foreach (KeyValuePair<string, object> loopPair in this.Dictionary)
            {
                fields += loopPair.Key;
                processed++;

                if (processed < this.Dictionary.Count)
                {
                    fields += ", ";
                }
            }

            fields += " ) VALUES ( ";

            processed = 0;

            foreach (KeyValuePair<string, object> loopPair in this.Dictionary)
            {
                fields += string.Format("'{0}'", loopPair.Value is bool ? (((bool)loopPair.Value) ? 1 : 0) : loopPair.Value);
                processed++;

                if (processed < this.Dictionary.Count)
                {
                    fields += ", ";
                }
            }

            fields += " )";

            Database.Execute("INSERT INTO {0} {1}", this.Table, fields);
        }

        public int InsertAndReturnID()
        {
            this.Insert();

            return (int)(ulong)Database.Scalar("SELECT LAST_INSERT_ID()");
        }

        public void Update(string constraints, params object[] args)
        {
            int processed = 0;

            string fields = string.Empty;

            foreach (KeyValuePair<string, object> loopPair in this.Dictionary)
            {
                fields += string.Format("{0}='{1}'", loopPair.Key, loopPair.Value is bool ? (((bool)loopPair.Value) ? 1 : 0) : loopPair.Value);
                processed++;

                if (processed < this.Dictionary.Count)
                {
                    fields += ", ";
                }
            }

            Database.Execute("UPDATE {0} SET {1} WHERE {2}", this.Table, fields, string.Format(constraints, args));
        }

        public override string ToString()
        {
            string result = this.Table + " [ ";

            int processed = 0;

            foreach (KeyValuePair<string, object> value in this.Dictionary)
            {
                result += value.Key;
                processed++;

                if (processed < this.Dictionary.Count)
                {
                    result += ", ";
                }
            }

            result += " ]";

            return result;
        }
    }
}
