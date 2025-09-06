namespace Ovation.Application.Common.Interfaces
{
    public interface IDomainEvents
    {
        Task NFTVisibilityChangedEvent(Guid userId);

        Task WalletAddedEvent(Guid userId, string address, string chain, Guid? walletTypeId);

        Task WalletDeletedEvent(Guid id, string action);

        Task XAccountConnectedEvent(Guid userId);

        Task UserReferredEvent(Guid userId, string refferal);

        Task TaskPerformedEvent(Guid userId, string task);

        Task WalletOwnershipVerifiedEvent(string address);
    }
}
