using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.SessionManager
{
    class SessionManager
    {
        private static SessionManager instance;
        public static SessionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SessionManager();
                return instance;
            }
        }

        private MemoryCache cache;

        private SessionManager()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void CreateSession(int accountId, string email)
        {
            var session = new Session(0, accountId, email, DateTime.Now);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

            cache.Set(0, session, cacheEntryOptions);
        }

        public bool SessionExists(int id) => cache.Get(id) != null;
        public Session GetSession(int id) => (Session)cache.Get(id);
    }
}
