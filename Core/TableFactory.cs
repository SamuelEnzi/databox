using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class TableFactory
    {
        public static string Create(string json, string table)
        {
            var data = json.Deserialize<JObject>();
            var columns = GetColumns(data);
            return GenerateSQL(columns, table);
        }

        public static string Create(JObject data, string table)
        {
            var columns = GetColumns(data);
            return GenerateSQL(columns, table);
        }

        private static string GenerateSQL(List<(string name, string type)> columns, string table)
        {
            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE {table} (");
            sb.Append("id INT UNSIGNED NOT NULL AUTO_INCREMENT,");
            sb.Append("created TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP(),");
            foreach (var (name, type) in columns)
                sb.Append($"{name} {type},");
            sb.Append("PRIMARY KEY (id));");
            return sb.ToString();
        }

        private static List<(string name, string type)> GetColumns(JObject data)
        {
            var temp = new List<(string name, string type)>();
            foreach (var property in data.Properties())
                temp.Add((property.Name, GetType(property.Value.Type)));
            return temp;
        }

        private static string GetType(JTokenType tokenType)
        {
            switch (tokenType)
            {
                case JTokenType.Integer:
                    return "INT";
                case JTokenType.Float:
                    return "FLOAT";
                case JTokenType.Boolean:
                    return "BOOLEAN";
                case JTokenType.String:
                    return "TEXT";
                default:
                    return "TEXT";
            }
        }
    }
}
