using apbd10.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd10.Context;

public partial class UserContext : DbContext
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }

    protected UserContext()
    {
    }
    public DbSet<User> Users { get; set; }
    
}