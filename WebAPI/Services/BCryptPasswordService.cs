namespace WebAPI.Services
{
    public class BCryptPasswordService : IPasswordService
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public bool Verify(string password, string passwordHash) =>
            !string.IsNullOrEmpty(password) &&
            !string.IsNullOrEmpty(passwordHash) &&
            BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
