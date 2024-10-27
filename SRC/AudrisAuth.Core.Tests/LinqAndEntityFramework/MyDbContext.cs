using Microsoft.EntityFrameworkCore;

namespace AudrisAuth.Core.Tests.LinqAndEntityFramework;

public class Person
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}

public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required Person Coach { get; set; }
    public int CoachId { get; set; }
}

public class YourDbContext : DbContext
{
    public DbSet<Person> People { get; set; }
    public DbSet<Team> Teams { get; set; }

    public YourDbContext(DbContextOptions<YourDbContext> options)
        : base(options)
    {
    }
}

