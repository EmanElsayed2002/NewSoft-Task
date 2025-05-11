namespace NewSoftTask.Application.DTOs.Authentication;

public record _RegisterRequest
(
    string Email,
    string Password,
    string FullName,
    string Address,
    string Phone,
    int Age,
    string Role
);
