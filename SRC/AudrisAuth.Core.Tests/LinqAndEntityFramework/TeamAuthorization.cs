namespace AudrisAuth.Core.Tests.LinqAndEntityFramework;
public class TeamAuthorization : DefaultAuthorization<Team>
{
    public TeamAuthorization()
    {
        // Define authorization rules
        AddGenericRule("Read", "true");
        AddGenericRule("Insert", "UserRoles.Contains(\"Manager\")");

        AddInstanceRule("Edit", "UserRoles.Contains(\"Manager\") || UserRoles.Contains(\"Admin\")");
        AddInstanceRule("Delete", "UserRoles.Contains(\"Admin\")");
        AddInstanceRule("StartTraining", "Resource.Coach.Name == UserId || UserClaims.Contains(\"TeamCoach:\"+Resource.Name)");
    }
}
