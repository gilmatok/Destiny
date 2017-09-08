using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Destiny
{
    public static class Application
    {
        public static readonly Random Random = new Random();

        public const short MapleVersion = 83;
        public const string PatchVersion = "1";
        public const byte Locale = 8;

        public const int DefaultBufferSize = 4096;

        public const char CommandIndiciator = '!';

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 }; // Raja
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 }; // nR0X

        public static string LaunchPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + @"\";
            }
        }

        public static string ExecutablePath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
            }
        }

        public static object CommandIndicator { get; set; }

        public static string ToCamel(this string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }

        public static bool IsAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!char.IsLetter(c) && !char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ToAlphaNumeric(this string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, "");
        }

        public static string ClearFormatters(this string value)
        {
            return value.Replace("{", "{{").Replace("}", "}}");
        }
    }
}