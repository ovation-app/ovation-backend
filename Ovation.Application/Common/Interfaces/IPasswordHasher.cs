namespace Ovation.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);

        string HashPassword(string password);
    }
}
