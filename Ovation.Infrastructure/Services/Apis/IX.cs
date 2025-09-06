using Ovation.Application.DTOs.X;
using Refit;

namespace Ovation.Persistence.Services.Apis
{
    internal interface IX
    {
        [Get("/2/users/{userId}/tweets")]
        Task<XUserTweetDto> GetUserTimelineAsync([AliasAs("userId")] string userId, [AliasAs("exclude")] string exclude = "replies,retweets");

        [Get("/2/users/by/username/{username}")]
        Task<XPublicMetricDto> GetUserPublicMetricsByUsernameAsync([AliasAs("username")] string username, [AliasAs("user.fields")] string userFields = "public_metrics");

        [Get("/2/users/{userId}")]
        Task<XPublicMetricDto> GetUserPublicMetricsByUserIdAsync([AliasAs("userId")] string username, [AliasAs("user.fields")] string userFields = "public_metrics");

        [Get("/2/users/by/username/{username}")]
        Task<XUserDto> GetUserByUsernameAsync([AliasAs("username")] string username);
    }
}