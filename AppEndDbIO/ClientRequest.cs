using System.Text.Json;

namespace AppEndDbIO
{
    public class ClientRequest
    {
        public string Id { set; get; } = "";
        public string Method { set; get; } = "";
        public JsonElement Inputs { set; get; }
      
    }
}
