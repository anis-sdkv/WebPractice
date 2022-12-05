
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    class AuthorizeInfo
    {
        public bool IsAuthorize { get; set; }
        public int Id { get; set; }

        public static AuthorizeInfo Parse(string info)
        {
            try
            {
                var splited = info.Split(',');
                var isAuthorized = bool.Parse(splited[0].Replace("IsAuthorize:", ""));
                var id = int.Parse(splited[1].Replace("Id=", ""));
                return new AuthorizeInfo() { IsAuthorize = isAuthorized, Id = id };
            }
            catch
            {
                    throw new Exception($"Incorrect AuthorizeInfo format: {info}");
            }
        }
    }
}
