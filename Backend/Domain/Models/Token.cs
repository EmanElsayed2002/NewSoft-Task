using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    [Owned]
    public class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpired { get; set; }
        public DateTime RefreshTokenExpired { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsExpired => DateTime.UtcNow > AccessTokenExpired;
    }
}
