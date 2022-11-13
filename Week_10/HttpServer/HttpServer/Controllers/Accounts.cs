using Azure;
using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM.DAO;
using HttpServer.ServerLogic;
using System.Net;

namespace HttpServer.Controllers
{
    [HttpController("accounts")]
    class Accounts
    {
        private AccountDAO _accountDAO =
            new AccountDAO("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");


        [HttpGET]
        public ResponseBuilder GetAccounts(HttpListenerResponse response)
        {
            var accounts = _accountDAO.GetAll();
            return new ResponseBuilder(response)
                .SetSerializable(accounts);
        }

        [HttpGET("\\d+")]
        public ResponseBuilder GetAccountById(int id, HttpListenerResponse response)
        {
            var account = _accountDAO.GetEntityByKey(id);
            return new ResponseBuilder(response)
                .SetSerializable(account);
        }

        [HttpPOST]
        public ResponseBuilder SaveAccount(string login, string password, HttpListenerResponse response)
        {
            var builder = new ResponseBuilder(response);
            if (_accountDAO.GetEntityByLogin(login) != null)
                return builder.SetMessage("Аккаунт уже зарегистрирован!");
            _accountDAO.Create(new Account(login, password));
            return builder.SetRedirect("https://steamcommunity.com/");
        }

        [HttpPOST]
        public ResponseBuilder Login(string email, string password, HttpListenerResponse response)
        {
            var builder = new ResponseBuilder(response);
            var account = _accountDAO.GetEntityByLogin(email);
            if (account == null) return builder;
            if (account.Password == password)
            {
                var id = account.Id;
                var cookie = new Cookie("SessionId", $"IsAuthorize:true,id={id}");
                builder.SetCookie(cookie);
                return builder;
            }
            return builder.SetMessage("Н");
        }
    }
}
