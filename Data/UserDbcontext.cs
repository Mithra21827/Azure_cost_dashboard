using azure_cost_dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace azure_cost_dashboard.Data
{
    public class UserDbContext: DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options):base(options) { }

        public DbSet<User> users { get; set; }
    }
}
