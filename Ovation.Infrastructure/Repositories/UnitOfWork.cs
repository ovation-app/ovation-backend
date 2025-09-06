using Ovation.Application.Repositories;
using Ovation.Persistence.Data;

namespace Ovation.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public readonly OvationDbContext _context;

    public UnitOfWork(OvationDbContext context)
    {
        _context = context;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken = default);
    }

    public async Task BeginTransactionAsync()
    {
        var transaction = _context.Database.CurrentTransaction;

        if (transaction == null)
            await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}