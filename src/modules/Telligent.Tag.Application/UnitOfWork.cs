using Telligent.Core.Domain.Repositories;
using Telligent.Core.Infrastructure.Database;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application;

/// <summary>
/// UnitOfWork
/// </summary>
public class UnitOfWork : IDisposable
{
    private bool _disposed;

    public UnitOfWork(
        BaseDbContext context,
        IRepository<BatchTransactionLog> batchTransactionLogRepository,
        IRepository<BehaviorTagCategory> behaviorTagCategoryRepository,
        IRepository<CustomizationTagCategory> customizationTagCategoryRepository,
        IRepository<Event> eventRepository,
        IRepository<SystemEvent> systemEventRepository,
        IRepository<EventTag> eventTagRepository,
        IRepository<Domain.Tags.Tag> tagRepository,
        IRepository<TagCategoryPermission> tagCategoryPermissionRepository,
        IRepository<TagTracking> tagTrackingRepository)
    {
        Context = context;
        BatchTransactionLogRepository = batchTransactionLogRepository;
        BehaviorTagCategoryRepository = behaviorTagCategoryRepository;
        CustomizationTagCategoryRepository = customizationTagCategoryRepository;
        EventRepository = eventRepository;
        SystemEventRepository = systemEventRepository;
        EventTagRepository = eventTagRepository;
        TagRepository = tagRepository;
        TagCategoryPermissionRepository = tagCategoryPermissionRepository;
        TagTrackingRepository = tagTrackingRepository;
    }

    public IRepository<BatchTransactionLog> BatchTransactionLogRepository { get; }
    public IRepository<BehaviorTagCategory> BehaviorTagCategoryRepository { get; }
    public IRepository<CustomizationTagCategory> CustomizationTagCategoryRepository { get; }
    public IRepository<Event> EventRepository { get; }
    public IRepository<SystemEvent> SystemEventRepository { get; }
    public IRepository<EventTag> EventTagRepository { get; }
    public IRepository<Domain.Tags.Tag> TagRepository { get; }
    public IRepository<TagCategoryPermission> TagCategoryPermissionRepository { get; }
    public IRepository<TagTracking> TagTrackingRepository { get; }

    /// <summary>
    /// Context
    /// </summary>
    public BaseDbContext Context { get; private set; }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// SaveChange
    /// </summary>
    /// <returns></returns>
    public async Task<int> SaveChangeAsync()
    {
        return await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
            {
                Context.Dispose();
                Context = null;
            }

        _disposed = true;
    }
}