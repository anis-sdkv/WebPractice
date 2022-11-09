using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM.Repository
{
    public interface IRepository
    {
        void Add(Account account);
        void Remove(Account account);
        void Update(Account account); 
    }
}
