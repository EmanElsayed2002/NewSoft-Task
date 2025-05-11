namespace NewSoftTask.Application.DTOs.Authentication;

public record ConfirmEmailRequest
(
    string UserId,
    string Code

);
