using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int Age { get; set; }
        public ICollection<Token> tokens { get; set; }
        public ICollection<UserSubject> studentSubjects { get; set; }
    }
}
