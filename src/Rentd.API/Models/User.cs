namespace Rentd.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string UserName { get; set; }
        public string AccountLocked { get; set; }
        public string LoginAttempts { get; set; }
    }
}