namespace Rentd.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string UserName { get; set; }
    }
}