using System;

namespace Models.RequestModels
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expiry { get; set; }
    }
}