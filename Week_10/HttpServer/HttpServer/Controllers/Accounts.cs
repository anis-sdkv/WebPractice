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
        public ResponseBuilder GetAccounts(HttpListenerContext context)
        {
            var builer = new ResponseBuilder(context.Response);
            if (context.Request.Cookies
                .FirstOrDefault(c => c.Name == "SessionId" &&
                c.Value.StartsWith("IsAuthorize:true")) == null)
                return builer.SetStatusCode(401);
            var accounts = _accountDAO.GetAll();
            return builer.SetObject(accounts);
        }

        [HttpGET("\\d+")]
        public ResponseBuilder GetAccountById(int id, HttpListenerContext context)
        {
            var account = _accountDAO.GetEntityByKey(id);
            return new ResponseBuilder(context.Response)
                .SetObject(account);
        }

        [HttpPOST]
        public ResponseBuilder SaveAccount(string login, string password, HttpListenerContext context)
        {
            var builder = new ResponseBuilder(context.Response);
            if (_accountDAO.GetEntityByLogin(login) != null)
                return builder.SetMessage("Аккаунт с таким логином уже зарегистрирован!");
            _accountDAO.Create(new Account(login, password));
            return builder.SetRedirect("https://steamcommunity.com/");
        }

        [HttpPOST]
        public ResponseBuilder Login(string email, string password, HttpListenerContext context)
        {
            var builder = new ResponseBuilder(context.Response);
            var account = _accountDAO.GetEntityByLogin(email);
            if (account == null) return builder;
            if (account.Password == password)
            {
                var id = account.Id;
                var cookie = new Cookie("SessionId", $"IsAuthorize:true,id={id}");
                builder.SetCookie(cookie);
                return builder;
            }
            return builder.SetMessage("Пожалуйста, проверьте свой пароль и имя аккаунта и попробуйте снова.");
        }
    }
}
