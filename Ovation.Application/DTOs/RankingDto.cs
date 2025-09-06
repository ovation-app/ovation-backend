using Microsoft.EntityFrameworkCore;

namespace Ovation.Application.DTOs
{
    [Keyless]
    public class RankingDto
    {
        public string Value { get; set; } = string.Empty;

        public int Ranking { get; set; }

        public int UserCount { get; set; }
    }
}
