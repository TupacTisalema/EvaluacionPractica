using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GatewayResponse
    {
        public int StatusCode { get; set; }
        public required string Body { get; set; }
        public required IDictionary<string, string> Headers { get; set; }
    }
}
