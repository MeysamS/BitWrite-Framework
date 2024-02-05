using Bw.Core.Messaging;
using Bw.Core.Messaging.Persistence;
using System.Linq.Expressions;

namespace Bw.Messaging.Persistence;


public class NullMessagePersistenceRepository : IMessagePersistenceRepository
{
    Task IMessagePersistenceRepository.AddAsync(StoreMessage storeMessage, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IMessagePersistenceRepository.ChangeStateAsync(Guid messageId, MessageStatus status, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IMessagePersistenceRepository.CleanupMessages()
    {
        return Task.CompletedTask;
    }

    Task<IReadOnlyList<StoreMessage>> IMessagePersistenceRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<StoreMessage>>(new List<StoreMessage>()); // مثال: یک لیست خالی به عنوان نتیجه
    }

    Task<IReadOnlyList<StoreMessage>> IMessagePersistenceRepository.GetByFilterAsync(Expression<Func<StoreMessage, bool>> predicate, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<StoreMessage>>(new List<StoreMessage>());
    }

    Task<StoreMessage?> IMessagePersistenceRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<StoreMessage?>(null); // مثال: بازگشت یک مقدار خالی برای Nullable
    }

    Task<bool> IMessagePersistenceRepository.RemoveAsync(StoreMessage storeMessage, CancellationToken cancellationToken)
    {
        return Task.FromResult(true); // مثال: همیشه با موفقیت حذف شده تلقی شود
    }

    Task IMessagePersistenceRepository.UpdateAsync(StoreMessage storeMessage, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
