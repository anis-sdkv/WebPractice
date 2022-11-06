using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpPOST : HttpMethod
    {
        public HttpPOST(string methodURI) : base(methodURI) { }
        public HttpPOST() : base() { }
    }
}
