using Microsoft.EntityFrameworkCore;

namespace TaskManagementAPI;

public static class DbContextExtensions
{
    extension (DbContext dbContext)
    {
        public void AddOrUpdate<T>(params IEnumerable<T> entities) where T: class
        {
            dbContext.AttachRange(entities);

            foreach (var entity in entities)
            {
                var entry = dbContext.Entry<T>(entity);
                if (entry.GetDatabaseValues() is null)
                {
                    entry.State = EntityState.Added;
                }
                else
                {
                    entry.DetectChanges();
                }                
            }
        }
    }
}