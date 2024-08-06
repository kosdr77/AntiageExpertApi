using Microsoft.EntityFrameworkCore;
using UserManagementService.Domain.Models;

namespace UserManagementService.DataContexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
