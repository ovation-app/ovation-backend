namespace Ovation.Application.DTOs.ApiUser
{
    public class UserClaims
    {
        public Guid UserId { get; set; } = Guid.Empty!;

        public string Username { get; set; } = null!;

        public string? Email { get; set; }
    }
}
