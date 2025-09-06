namespace Ovation.Application.DTOs
{
    public class ResponseData
    {
        public string Message { get; set; } = "An error occurred";
        public object? Data { get; set; }
        public RankingDto? RankData { get; set; }
        public string? Cursor { get; set; } = null;
        public Guid GuidValue { get; set; } = Guid.Empty;
        public int IntValue { get; set; }
        public bool Status { get; set; } = false;
        public int StatusCode { get; set; } = 400;
    }
}
