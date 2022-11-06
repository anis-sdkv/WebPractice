using HttpServer.Attributes;
using HttpServer.Models;
using System.Xml.Linq;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace HttpServer.Controllers
{
    [HttpController("accounts")]
    public class Accounts
    {
        private string connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;";

        [HttpGET]
        public List<Account> GetAccounts()
        {
            var accounts = new List<Account>();
            var commandString = "SELECT * FROM Accounts";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(commandString, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accounts.Add(
                                new Account(reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2))
                                );
                        }
                    }
                }
            }
            return accounts;
        }

        [HttpGET("\\d+")]
        public Account? GetAccountById(int id)
        {
            var commandString = $"SELECT * FROM Accounts WHERE id = {id}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(commandString, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Account(reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2));
                    }
                }
            }
            return null;
        }

        [HttpPOST]
        public void SaveAccount(string login, string password)
        {
            var commandString = $"INSERT INTO Accounts VALUES ('{login}', '{password}')";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(commandString, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
