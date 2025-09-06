namespace Ovation.Application.DTOs
{
    public class UserWalletMetaData
    {
        public List<string> MultiChains { get; set; } = new();

        public bool IsMultiChain { get; set; } = false;
    }
}
