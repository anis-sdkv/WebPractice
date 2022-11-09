using HttpServer.Attributes;
using HttpServer.Models;
using System.Xml.Linq;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using HttpServer.ORM.DAO;

namespace HttpServer.Controllers
{
    [HttpController("accounts")]
    public class Accounts
    {

        private AccountDAO _accountDAO =
            new AccountDAO("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");
        [HttpGET]
        public List<Account> GetAccounts() =>
            _accountDAO.GetAll();

        [HttpGET("\\d+")]
        public Account? GetAccountById(int id) =>
            _accountDAO.GetEntityByKey(id);

        [HttpPOST]
        public void SaveAccount(string login, string password) =>
            _accountDAO.Create(new Account(login, password));
    }
}
