namespace Application.DTOs.User
{
    public class ConfirmEmailDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
