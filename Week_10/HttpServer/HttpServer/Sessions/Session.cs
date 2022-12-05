using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Sessions
{
    class Session
    {
        public Guid Guid { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public DateTime CreateDateTime { get; set; }

        public Session(Guid guid, int accountId, string email, DateTime time)
        {
            Guid = guid;
            AccountId = accountId;
            Email = email;
            CreateDateTime = time;
        }
    }
}
