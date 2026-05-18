using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventPlanner.Models
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // ⚡️ Локалната база за миграции
            optionsBuilder.UseSqlite("Data Source=LocalDB/eventplanner_local.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
