using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Models
{
    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public long ResponseTime { get; set; }
    }
}
