using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IDevRepository
    {
        Task<bool> VerifyCoreToken(Guid tokenId);

        Task<bool> VerifyToken(Guid tokenId);
    }
}
