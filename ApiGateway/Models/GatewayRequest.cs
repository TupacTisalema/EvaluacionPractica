using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class GatewayRequest
    {
        [Required]
        public string Method { get; set; } // Usar cadena en lugar de HttpMethod

        [Required]
        [Url]
        public string Url { get; set; }

        public string Body { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }
}
