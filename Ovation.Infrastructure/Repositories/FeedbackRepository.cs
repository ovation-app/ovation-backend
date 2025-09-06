using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Persistence.Services;
using UserFeedback = Ovation.Domain.Entities.UserFeedback;

namespace Ovation.Persistence.Repositories
{
    internal class FeedbackRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<UserFeedback>(serviceScopeFactory), IFeedbackRepository
    {
        public async Task<ResponseData> AddUserFeedbackAsync(UserFeedbackDto feedbackDto, Guid userId)
        {
            if (!HelperFunctions.IsValidEmail(feedbackDto.UserEmail))
                return new ResponseData { Message = "Invalid email" };

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var feedback = new UserFeedback
                {
                    UserId = userId.ToByteArray(),
                    Improvement = feedbackDto.Improvement,
                    LikelyRecommend = feedbackDto.LikelyRecommend,
                    Addition = feedbackDto.Addition,
                    BiggestPain = feedbackDto.BiggestPain,
                    Confusion = feedbackDto.Confusion,
                    Satisfactory = feedbackDto.Satisfactory,
                    UsefulFeature = feedbackDto.UsefulFeature,
                    UserEmail = feedbackDto.UserEmail
                };

                await _context.UserFeedbacks.AddAsync(feedback);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Feedback Saved!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();

                return new();
            }
        }
    }
}
