namespace YapYapAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BIO { get; set; } = string.Empty;
        public int status_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        public Status Status { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BIO { get; set; } = string.Empty;
        public int status_id { get; set; }
        public DateTime created_at { get; set; }
    }

    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BIO { get; set; } = string.Empty;
        public int status_id { get; set; }
    }

    public class LoginDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }
}