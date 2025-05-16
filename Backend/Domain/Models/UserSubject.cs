namespace Domain.Models
{
    public class UserSubject
    {

        public int UserId { get; set; }
        public int SubjectId { get; set; }

        public User User { get; set; }
        public Subject Subject { get; set; }
    }
}
