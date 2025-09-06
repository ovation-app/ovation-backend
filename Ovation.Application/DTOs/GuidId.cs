namespace Ovation.Application.DTOs
{
    public class GuidId
    {
        public Guid Id { get; set; } = Guid.Empty!;
    }

    public class IntId
    {
        public int Id { get; set; }
    }

    public class DeleteFavNft
    {
        public List<IntId> Data { get; set; } = new()!;
    }
}
