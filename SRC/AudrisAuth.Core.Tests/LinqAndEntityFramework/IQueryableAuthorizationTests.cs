using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace AudrisAuth.Core.Tests.LinqAndEntityFramework;

public class IQueryableAuthorizationTests
{
    private ClaimsPrincipal CreateUser(string name, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, name)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void Coach_CanOnly_StartTraining_On_Owns_Teams()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<YourDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "Luigi"),
            new Claim("TeamCoach", "Team C")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        var authorization = new TeamAuthorization();

        // Seed data
        using (var context = new YourDbContext(options))
        {
            var coachLuigi = new Person { Name = "Luigi" };
            var coachMario = new Person { Name = "Mario" };

            var team1 = new Team { Name = "Team A", Coach = coachLuigi };
            var team2 = new Team { Name = "Team B", Coach = coachMario };
            var team3 = new Team { Name = "Team C", Coach = coachMario };

            context.People.AddRange(coachLuigi, coachMario);
            context.Teams.AddRange(team1, team2, team3);
            context.SaveChanges();
        }

        // Act
        using (var context = new YourDbContext(options))
        {
            var authExpression = authorization.GetAuthorizationExpression(user, "StartTraining");

            var teams = context.Teams
                .Include(t => t.Coach)
                .Where(authExpression)
                .OrderBy(t => t.Name)
                .ToList();

            // Assert
            Assert.Equal(2, teams.Count);
            Assert.Equal("Team A", teams[0].Name);
            Assert.Equal("Team C", teams[1].Name);
        }
    }

    [Fact]
    public void Manager_Can_Read_All_Teams()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<YourDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var user = CreateUser("Peach", "Manager"); // Peach is a manager
        var authorization = new TeamAuthorization();

        // Seed data
        using (var context = new YourDbContext(options))
        {
            var coachLuigi = new Person { Name = "Luigi" };
            var coachMario = new Person { Name = "Mario" };

            var team1 = new Team { Name = "Team A", Coach = coachLuigi };
            var team2 = new Team { Name = "Team B", Coach = coachMario };

            context.People.AddRange(coachLuigi, coachMario);
            context.Teams.AddRange(team1, team2);
            context.SaveChanges();
        }

        // Act
        using (var context = new YourDbContext(options))
        {
            var authExpression = authorization.GetAuthorizationExpression(user, "Read");

            var teams = context.Teams
                .Include(t => t.Coach)
                .Where(authExpression)
                .ToList();

            // Assert
            Assert.Equal(2, teams.Count);
        }
    }
}
