using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Portfolio.Domain.Interface;
using Portfolio.Domain.Interface.Markers;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly Stack<IDbContextTransaction> _transactions = new();

        public UnitOfWork(AppDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public void Begin()
        {
        }

        public async Task<int> Confirm(CancellationToken cancellationToken = default)
        {
            AplicarAuditoria();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransaction(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _transactions.Push(transaction);
        }

        public async Task ConfirmTransaction(CancellationToken cancellationToken = default)
        {
            if (_transactions.Count == 0)
                throw new InvalidOperationException("Nenhuma transação ativa para confirmar.");

            var transaction = _transactions.Pop();
            try
            {
                AplicarAuditoria();
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        public async Task RejectTransaction(CancellationToken cancellationToken = default)
        {
            if (_transactions.Count == 0) return;

            var transaction = _transactions.Pop();
            try
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        private void AplicarAuditoria()
        {
            var agora = DateTime.UtcNow;
            var userId = _userResolver.GetCurrentUserId();

            foreach (var entry in _context.ChangeTracker.Entries<IMustAudited>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreationTime = agora;
                        entry.Entity.CreationUserId = userId;
                        entry.Entity.ModificationTime = null;
                        entry.Entity.ModificationUserId = null;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModificationTime = agora;
                        entry.Entity.ModificationUserId = userId;
                        entry.Property(nameof(IMustAudited.CreationTime)).IsModified = false;
                        entry.Property(nameof(IMustAudited.CreationUserId)).IsModified = false;
                        break;
                }
            }
        }

        public void Dispose()
        {
            while (_transactions.Count > 0)
                _transactions.Pop().Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            while (_transactions.Count > 0)
                await _transactions.Pop().DisposeAsync();
        }
    }
}
