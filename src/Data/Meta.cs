using System.Collections.Generic;

namespace Destiny.Data
{
    public static class Meta
    {
        public static Dictionary<string, Dictionary<string, Column>> Tables { get; private set; }

        public static void Initialize(bool mcdb)
        {
            Meta.Tables = new Dictionary<string, Dictionary<string, Column>>();

            foreach (Datum datum in new Datums("COLUMNS").Populate("TABLE_SCHEMA = '{0}'{1}", Database.DefaultSchema, mcdb ? " OR TABLE_SCHEMA = 'mcdb'" : string.Empty))
            {
                Meta.Add(datum);
            }

            Log.Inform("Meta analyzed database.");
        }

        private static void Add(Datum datum)
        {
            Dictionary<string, Column> table;

            string tableName = (string)datum["TABLE_NAME"];

            if (Meta.Tables.ContainsKey(tableName))
            {
                table = Meta.Tables[tableName];
            }
            else
            {
                table = new Dictionary<string, Column>();

                Meta.Tables.Add(tableName, table);
            }

            table.Add((string)datum["COLUMN_NAME"], new Column(datum));
        }

        public static bool IsBool(string tableName, string fieldName)
        {
            return Meta.Tables[tableName][fieldName].ColumnType == "tinyint(1) unsigned";
        }

        public static bool IsDate(string tableName, string fieldName)
        {
            return Meta.Tables[tableName][fieldName].ColumnType == "date";
        }

        public static bool IsDateTime(string tableName, string fieldName)
        {
            return Meta.Tables[tableName][fieldName].ColumnType == "datetime";
        }
    }

    public sealed class Column
    {
        public string Name { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsUniqueKey { get; private set; }
        public string ColumnType { get; private set; }

        public Column(Datum datum)
        {
            this.Name = (string)datum["COLUMN_NAME"];
            this.IsPrimaryKey = (string)datum["COLUMN_KEY"] == "PRI";
            this.IsUniqueKey = (string)datum["COLUMN_KEY"] == "UNI";
            this.ColumnType = (string)datum["COLUMN_TYPE"];
        }
    }
}
