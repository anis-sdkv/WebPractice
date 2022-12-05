using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Sessions
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

        public Session CreateSession(int accountId, string email)
        {
            var session = new Session(Guid.NewGuid(), accountId, email, DateTime.Now);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

            cache.Set(session.Guid, session, cacheEntryOptions);
            return session;
        }

        public bool SessionExists(Guid guid) => cache.Get(guid) != null;
        public Session GetSession(Guid guid) => (Session)cache.Get(guid);
        public bool TryGetSession(Guid guid, out Session session)
        {
            return cache.TryGetValue(guid, out session);
        }
    }
}
