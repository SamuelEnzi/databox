using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public static class StatementFactory
    {
        public static string Create(this string json, string table)
        {
            var data = json.Deserialize<JObject>();
            var columns = GetColumns(data);
            return GenerateSQL(columns, table);
        }

        public static string Create(this JObject data, string table)
        {
            var columns = GetColumns(data);
            return GenerateSQL(columns, table);
        }

        private static string GenerateSQL(List<(string column, string value)> columns, string table)
        {
            var sb = new StringBuilder();
            sb.Append($"INSERT INTO {table} (");
            var columnNames = columns.Select(x => x.column);
            sb.Append(string.Join(",", columnNames));
            sb.Append(") VALUES (");
            var values = columns.Select(x => $"{x.value}");
            sb.Append(string.Join(",", values));
            sb.Append(");");
            return sb.ToString();
        }

        private static string EscapeString(string input)
        {
            if (input == null) return "NULL";
            return "'" + input.Replace("'", "\\'") + "'";
        }

        private static List<(string column, string value)> GetColumns(JObject data)
        {
            var temp = new List<(string name, string value)>();
            foreach (var property in data.Properties())
                temp.Add((property.Name, GetValue(property.Value.Type, property.Value)));
            return temp;
        }

        private static string GetValue(JTokenType type, JToken value)
        {
            switch (type)
            {
                case JTokenType.Integer:
                    return value.ToString();
                case JTokenType.Float:
                    return value.ToString();
                case JTokenType.Boolean:
                    return ConvertToBool(value.ToString());
                case JTokenType.String:
                    return EscapeString(value.ToString());
                default:
                    return EscapeString(value.ToString());
            }
        }

        private static string ConvertToBool(string v)
        {
            switch (v.ToLower().Trim())
            {
                case "true":
                    return "1";
                case "1":
                    return "1";
                default:
                    return "0";
            }
        }
    }
}
