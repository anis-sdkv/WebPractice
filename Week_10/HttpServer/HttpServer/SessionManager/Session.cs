using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.SessionManager
{
    class Session
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public DateTime CreateDateTime { get; set; }

        public Session( int id, int accountId, string email, DateTime time)
        {
            Id = id;
            AccountId = accountId;
            Email = email;
            CreateDateTime= time;
        }
    }
}
