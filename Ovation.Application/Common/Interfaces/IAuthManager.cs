namespace Ovation.Application.Common.Interfaces
{
    public interface IAuthManager
    {
        string CreateToken();

        Task<object?> GetUserDataAsync(byte[] userId);
    }
}
