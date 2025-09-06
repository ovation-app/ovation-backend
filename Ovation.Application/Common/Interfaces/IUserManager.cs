using Ovation.Application.DTOs.ApiUser;

namespace Ovation.Application.Common.Interfaces
{
    public interface IUserManager
    {
        UserPayload? GetUserPayload();
    }
}
