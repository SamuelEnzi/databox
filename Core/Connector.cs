using System.Threading.Tasks;
using MySqlConnector;

namespace Core
{
    public class Connector
    {
        private MySqlConnection connection;
        public Connector(string server, string username, string password, string database)
        {
            connection = new MySqlConnection($"Server={server};User ID={username};Password={password};Database={database}");
            connection.Open();
        }

        public async Task ExecuteAsync(string sql)
        {
            var command = new MySqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<MySqlDataReader> Fetch(string sql)
        {
            var command = new MySqlCommand(sql, connection);
            return await command.ExecuteReaderAsync();
        }

        ~Connector()
        {
            connection.Close();
        }
    }
}
