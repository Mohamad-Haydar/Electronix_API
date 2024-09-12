namespace Web_API.Configuration
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
    }
}