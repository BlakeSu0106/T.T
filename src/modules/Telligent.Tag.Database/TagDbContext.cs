using Microsoft.EntityFrameworkCore;
using Telligent.Core.Infrastructure.Database;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Database;

public class TagDbContext : BaseDbContext
{
    public TagDbContext(DbContextOptions<TagDbContext> options) : base(options)
    {
    }

    public DbSet<BatchTransactionLog> BatchTransactionLogs { get; set; }
    public DbSet<BehaviorTagCategory> BehaviorTagCategories { get; set; }
    public DbSet<CustomizationTagCategory> CustomizationTagCategories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventTag> EventTags { get; set; }
    public DbSet<SystemEvent> SystemEvents { get; set; }
    public DbSet<Domain.Tags.Tag> Tags { get; set; }
    public DbSet<TagCategoryPermission> TagCategoryPermissions { get; set; }
    public DbSet<TagTracking> TagTracking { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<BehaviorTagCategory>().Ignore(t => t.TenantId);
        builder.Entity<SystemEvent>().Ignore(t => t.TenantId);

        base.OnModelCreating(builder);
    }
}