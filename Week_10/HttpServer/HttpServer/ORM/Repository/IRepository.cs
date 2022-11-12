using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM.Repository
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Remove(T entity);
        void Update(T entity); 
    }
}
