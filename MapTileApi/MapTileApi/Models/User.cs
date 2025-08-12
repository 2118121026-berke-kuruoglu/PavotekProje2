namespace MapTileApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
    }

    public class UserRegisterDto
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class UserLoginDto
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
