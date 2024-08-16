using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.DDD;

namespace Shared.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		UpdateEntities(eventData.Context);

		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
	{
		UpdateEntities(eventData.Context);

		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void UpdateEntities(DbContext? context)
	{
		if (context == null) return;

		foreach(var entry in context.ChangeTracker.Entries<IEntity>())
		{
			if (entry.State.Equals(EntityState.Added))
			{
				entry.Entity.CreatedBy = "berkay";
				entry.Entity.CreatedAt = DateTime.UtcNow;
			}

			if (entry.State.Equals(EntityState.Added) || entry.State.Equals(EntityState.Modified) || entry.HasChangedOwnedEntities())
			{
				entry.Entity.LastModifiedBy = "berkay";
				entry.Entity.LastModified = DateTime.UtcNow;
			}
		}
	}
}

public static class Extensions
{
	public static bool HasChangedOwnedEntities(this EntityEntry entry) => entry.References.Any(r => 
	r.TargetEntry != null &&
	r.TargetEntry.Metadata.IsOwned() &&
	(r.TargetEntry.State is EntityState.Added || r.TargetEntry.State is EntityState.Modified);
}
