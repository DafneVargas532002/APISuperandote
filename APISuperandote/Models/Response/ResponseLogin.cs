namespace APISuperandote.Models.Response
{
    public class ResponseLogin
    {
        public int success { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
        public string? token { get; set; } // Nueva propiedad para el token

        public ResponseLogin()
        {
            this.success = 0;
        }
    }
}

