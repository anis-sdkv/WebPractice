using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public abstract class HttpMethod : Attribute
    {
        public string MethodURI;
        public HttpMethod(string methodURI)
        {
            MethodURI = methodURI;
        }
        public HttpMethod()
        {
            MethodURI = "^$";
        }
    }
}
