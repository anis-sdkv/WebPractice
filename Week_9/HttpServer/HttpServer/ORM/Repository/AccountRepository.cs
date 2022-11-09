using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM.Repository
{
    class AccountRepository : IRepository
    {
        private MyORM orm;
        public AccountRepository(MyORM orm)
        {
            this.orm = orm;
        }

        public void Add(Account account) =>
            orm.Insert(account);

        public void Remove(Account account) =>
            orm.Delete($"Id = {account.Id}");

        public void Update(Account account) =>
            orm.Update(account, $"Id = {account.Id}");
    }
}
