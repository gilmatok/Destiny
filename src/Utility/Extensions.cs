using System;
using System.Collections.Generic;

namespace Destiny.Utility
{
    public static class Extensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> pThis, TKey pKey, TValue pDefault)
        {
            TValue result;
            return pThis.TryGetValue(pKey, out result) ? result : pDefault;
        }

        public static bool GetBool(this DatabaseQuery query, string field)
        {
            return Extensions.GetByte(query, field) == 1 ? true : false;
        }

        public static byte GetByte(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? (byte)0 : (byte)query[field];
        }

        public static sbyte GetSByte(this DatabaseQuery query, string field)
        {
            return query[field] == DBNull.Value ? (sbyte)0 : (sbyte)query[field];
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
}
