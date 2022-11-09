using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM.DAO
{
    interface IDAO<TEntity, TKey>
    {
        public List<TEntity> GetAll();
        public TEntity? GetEntityByKey(TKey key);
        public bool Update(TEntity entity);
        public int Delete(TKey key);
        public bool Create(TEntity entity);
    }
}
