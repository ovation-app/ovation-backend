using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IFeedbackRepository
    {
        Task<ResponseData> AddUserFeedbackAsync(UserFeedbackDto feedbackDto, Guid userId);
    }
}
