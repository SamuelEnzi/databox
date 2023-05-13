using Core;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCollector
{
    public class Collector
    {
        private Config config;
        private Connector connector;
        private bool tableChecked = false;

        public Collector(Config config) 
        {
            this.config = config;
            this.connector = new Connector(config.Server, config.Username, config.Password, config.Database);
        }

        public async Task<bool> Collect(object set)
        {
            try
            {
                var data = JObject.FromObject(set);
                if (!tableChecked)
                {
                    var table_added = await CheckTable(data);
                    if (!table_added)
                        return false;
                }

                var sql = StatementFactory.Create(data, config.Dataset);
                await connector.ExecuteAsync(sql);
                return true;
            }
            catch { return false; }
        }

        private async Task<bool> CheckTable(JObject set)
        {
            var reader = await connector.Fetch("show tables");
            List<string> tables = new List<string>();
            while (reader.Read())
            {
                var table = reader.GetString(0);
                tables.Add(table);
            }
            reader.Close();

            var foundTable = false;

            foreach (var table in tables)
                if (table.ToLower().Trim() == config.Dataset.ToLower().Trim())
                    foundTable = true;
            
            try
            {
                if (!foundTable)
                {
                    var sql = TableFactory.Create(set, config.Dataset);
                    await connector.ExecuteAsync(sql);
                }
            }
            catch { return false; }
         
            tableChecked = true;
            return true;
        }
    }
}
