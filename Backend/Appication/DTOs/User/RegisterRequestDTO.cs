namespace Application.DTOs.User
{
    public class RegisterRequestDTO
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }

        public string Role { get; set; }

    }
}
