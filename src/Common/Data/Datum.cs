using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Destiny.Data
{
    public sealed class Datums : IEnumerable<Datum>
    {
        private string Table { get; set; }
		private string TableAlias { get; set; }
        private List<Datum> Values { get; set; }
        private string ConnectionString { get; set; }
		private List<string> JoinConditions { get; set; }
		private List<MySqlParameter> JoinParameters { get; set; }
		public int Count
		{
			get { return this.Values.Count; }
		}

		public Datum this[int index]
		{
			get { return Values[index]; }
		}


		public Datums(string table) : this(table, Database.Schema) {}

        public Datums(string table, string schema)
        {
			var separated = SeparateTableAndAlias(table);
			this.Table = separated.Item1;
			this.TableAlias = separated.Item2;
			this.ConnectionString = string.Format("server={0}; database={1}; uid={2}; password={3}; convertzerodatetime=yes;",
                Database.Host,
                schema,
                Database.Username,
                Database.Password);
			this.JoinConditions = new List<string>();
			this.JoinParameters = new List<MySqlParameter>();
		}

        private void PopulateInternal(string fields, string constraints, params object[] args)
        {
            this.Values = new List<Datum>();

            using (MySqlConnection connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = Database.GetCommand(connection, constraints, args))
                {
					string tableAndAlias = string.Format("`{0}`{1}", this.Table, !string.IsNullOrWhiteSpace(this.TableAlias) ? string.Format(" AS `{0}`", this.TableAlias) : "");
					string joinClause = string.Join("", this.JoinConditions);
					foreach (var param in this.JoinParameters)
					{
						command.Parameters.Add(param);
					}
					string whereClause = constraints != null ? " WHERE " + command.CommandText : string.Empty;
					command.CommandText = string.Format("SELECT {0} FROM {1}{2}{3}", fields == null ? "*" : Database.CorrectFields(fields), tableAndAlias, joinClause, whereClause);

					using (MySqlDataReader reader = command.ExecuteReader())
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
            }
        }

		private Tuple<string, string> SeparateTableAndAlias(string tableAndAlias)
		{
			// In case table is aliased, split it so that both the table and alias have separate quotes
			int spacePos = tableAndAlias.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
			string tableName = spacePos > 0 ? tableAndAlias.Substring(0, spacePos) : tableAndAlias;
			string tableAlias = spacePos > 0 ? tableAndAlias.Substring(spacePos + 1) : "";
			tableAlias = tableAlias.StartsWith("AS ", StringComparison.InvariantCultureIgnoreCase) ? tableAlias.Remove(0, 3) : tableAlias;

			return new Tuple<string, string>(tableName, tableAlias);
		}

		private Datums JoinInternal(string table, string condition, string joinType = null, params object[] args)
		{
			StringBuilder outputBuilder = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(joinType))
			{
				outputBuilder.Append(string.Format(" {0}", joinType.ToUpper()));
			}

			var tableAndAlias = SeparateTableAndAlias(table);
			string safeTableAndAlias = string.Format("`{0}`{1}", tableAndAlias.Item1, !string.IsNullOrWhiteSpace(tableAndAlias.Item2) ? string.Format(" AS `{0}`", tableAndAlias.Item2) : "");
			outputBuilder.Append(string.Format(" JOIN {0}", safeTableAndAlias));

			if (!String.IsNullOrWhiteSpace(condition))
			{
				outputBuilder.Append(string.Format(" ON {0}", Database.ParameterizeCommandText("join" + this.JoinConditions.Count + "_", condition, args)));
				this.JoinParameters.AddRange(Database.ConstraintsToParameters("join" + this.JoinConditions.Count + "_", condition, args));
			}

			this.JoinConditions.Add(outputBuilder.ToString());

			return this;
		}

		public Datums Join(string table, string condition, params object[] args)
		{
			return this.JoinInternal(table, condition, null, args);
		}

		public Datums LeftJoin(string table, string condition, params object[] args)
		{
			return this.JoinInternal(table, condition, "LEFT", args);
		}

		public Datums FullJoin(string table, string condition, params object[] args)
		{
			return this.JoinInternal(table, condition, "FULL", args);
		}

		public Datums Populate()
        {
            this.PopulateInternal(null, null, null);

            return this;
        }

        public Datums Populate(string constraints, params object[] args)
        {
            this.PopulateInternal(null, constraints, args);

            return this;
        }

        public Datums PopulateWith(string fields)
        {
            this.PopulateInternal(fields, null, null);

            return this;
        }

        public Datums PopulateWith(string fields, string constraints, params object[] args)
        {
            this.PopulateInternal(fields, constraints, args);

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
		public string TableAlias { get; private set; }
        public Dictionary<string, Object> Dictionary { get; set; }
        private string ConnectionString { get; set; }
		private List<string> JoinConditions { get; set; }
		private List<MySqlParameter> JoinParameters { get; set; }

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

        public Datum(string table) : this(table, Database.Schema, new Dictionary<string, object>()) {}

        public Datum(string table, string schema) : this(table, schema, new Dictionary<string, object>()) {}

        public Datum(string table, Dictionary<string, object> dictionary) : this(table, Database.Schema, dictionary) {}

        public Datum(string table, string schema, Dictionary<string, object> dictionary)
        {
			var separated = SeparateTableAndAlias(table);
			this.Table = separated.Item1;
			this.TableAlias = separated.Item2;
			this.Dictionary = dictionary;
            this.ConnectionString = string.Format("server={0}; database={1}; uid={2}; password={3}; convertzerodatetime=yes;",
                Database.Host,
                schema,
                Database.Username,
                Database.Password);
			this.JoinConditions = new List<string>();
			this.JoinParameters = new List<MySqlParameter>();
		}

        public Datum Populate(string constraints, params object[] args)
        {
            this.PopulateWith("*", constraints, args);

            return this;
        }

        public Datum PopulateWith(string fields, string constraints, params object[] args)
        {
			using (MySqlConnection connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = Database.GetCommand(connection, constraints, args))
                {
					string tableAndAlias = string.Format("`{0}`{1}", this.Table, !string.IsNullOrWhiteSpace(this.TableAlias) ? string.Format(" AS `{0}`", this.TableAlias) : "");
					string joinClause = string.Join("", this.JoinConditions);
					foreach (var param in this.JoinParameters)
					{
						command.Parameters.Add(param);
					}
					string whereClause = constraints != null ? " WHERE " + command.CommandText : string.Empty;
					command.CommandText = string.Format("SELECT {0} FROM {1}{2}{3}", fields == null ? "*" : Database.CorrectFields(fields), tableAndAlias, joinClause, whereClause);

                    using (MySqlDataReader reader = command.ExecuteReader())
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
            }

            return this;
        }

		private Tuple<string, string> SeparateTableAndAlias(string tableAndAlias)
		{
			// In case table is aliased, split it so that both the table and alias have separate quotes
			int spacePos = tableAndAlias.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
			string tableName = spacePos > 0 ? tableAndAlias.Substring(0, spacePos) : tableAndAlias;
			string tableAlias = spacePos > 0 ? tableAndAlias.Substring(spacePos + 1) : "";
			tableAlias = tableAlias.StartsWith("AS ", StringComparison.InvariantCultureIgnoreCase) ? tableAlias.Remove(0, 3) : tableAlias;

			return new Tuple<string, string>(tableName, tableAlias);
		}

		private Datum JoinInternal(string table, string condition, string joinType = null, params object[] args)
		{
			StringBuilder outputBuilder = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(joinType))
			{
				outputBuilder.Append(string.Format(" {0}", joinType.ToUpper()));
			}

			var tableAndAlias = SeparateTableAndAlias(table);
			string safeTableAndAlias = string.Format("`{0}`{1}", tableAndAlias.Item1, !string.IsNullOrWhiteSpace(tableAndAlias.Item2) ? string.Format(" AS `{0}`", tableAndAlias.Item2) : "");
			outputBuilder.Append(string.Format(" JOIN {0}", safeTableAndAlias));

			if (!String.IsNullOrWhiteSpace(condition))
			{
				outputBuilder.Append(string.Format(" ON {0}", Database.ParameterizeCommandText("join" + this.JoinConditions.Count + "_", condition, args)));
				this.JoinParameters.AddRange(Database.ConstraintsToParameters("join" + this.JoinConditions.Count + "_", condition, args));
			}

			this.JoinConditions.Add(outputBuilder.ToString());

			return this;
		}

		public Datum Join(string table, string condition, params object[] args)
		{
			return this.JoinInternal(table, condition, null, args);
		}

		public Datum LeftJoin(string table, string condition, params object[] args)
		{
			return this.JoinInternal(table, condition, "LEFT", args);
		}

		public Datum FullJoin(string table, string condition, params object[] args)
		{
			return this.JoinInternal(table, condition, "FULL", args);
		}

		public void Insert()
        {
            string fields = "( ";

            int processed = 0;

            foreach (KeyValuePair<string, object> loopPair in this.Dictionary)
            {
                fields += "`" + loopPair.Key + "`";
                processed++;

                if (processed < this.Dictionary.Count)
                {
                    fields += ", ";
                }
            }

            fields += " ) VALUES ( ";

            object[] valueArr = new object[this.Dictionary.Count];
            this.Dictionary.Values.CopyTo(valueArr, 0);
            for (int i = 0; i < valueArr.Length; i++)
            {
                fields += "{" + i + "}";

                if (i < valueArr.Length - 1)
                {
                    fields += ", ";
                }
            }

            fields += " )";

            Database.Execute(string.Format("INSERT INTO `{0}` {1}", this.Table, fields), valueArr);
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

            object[] valueArr = new object[this.Dictionary.Count];
            this.Dictionary.Values.CopyTo(valueArr, 0);
            foreach (KeyValuePair<string, object> loopPair in this.Dictionary)
            {
                fields += string.Format("`{0}`={1}", loopPair.Key, "{" + processed + "}");
                processed++;

                if (processed < this.Dictionary.Count)
                {
                    fields += ", ";
                }
            }

            using (MySqlConnection connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = Database.GetCommand(connection, constraints, args))
                {
                    command.CommandText = Database.ParameterizeCommandText("set", string.Format("UPDATE `{0}` SET {1} WHERE ", this.Table, fields), valueArr) + command.CommandText;
                    command.Parameters.AddRange(Database.ConstraintsToParameters("set", fields, valueArr));

                    command.ExecuteNonQuery();
                }
            }
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
