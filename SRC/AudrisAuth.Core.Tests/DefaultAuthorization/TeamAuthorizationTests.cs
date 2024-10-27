using System.Security.Claims;

using AudrisAuth.Exceptions;

namespace AudrisAuth.Core.Tests.DefaultAuthorization;
public class TeamAuthorizationTests
{
    private readonly TeamAuthorization _authorization;
    private readonly Team _sampleResource;

    public TeamAuthorizationTests()
    {
        _authorization = new TeamAuthorization();
        _sampleResource = new Team("Black Dogs", new Person("Luigi"));
    }

    private ClaimsPrincipal CreateUserWithRoles(params string[] roles)
    {
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private ClaimsPrincipal CreateUserWithNameAndRoles(string name, params string[] roles)
    {
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, name));
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void Can_Read_Should_Return_True_For_Any_User()
    {
        // Arrange
        var user = CreateUserWithRoles(); // User with no roles

        // Act
        var canRead = _authorization.Can(user, TeamAuthorization.Actions.Read);

        // Assert
        Assert.True(canRead);
    }

    [Fact]
    public void Can_Insert_Should_Return_True_For_Manager()
    {
        // Arrange
        var user = CreateUserWithRoles("Manager");

        // Act
        var canInsert = _authorization.Can(user, TeamAuthorization.Actions.Insert);

        // Assert
        Assert.True(canInsert);
    }

    [Fact]
    public void Can_Insert_Should_Return_False_For_Non_Manager()
    {
        // Arrange
        var user = CreateUserWithRoles("User");

        // Act
        var canInsert = _authorization.Can(user, TeamAuthorization.Actions.Insert);

        // Assert
        Assert.False(canInsert);
    }

    [Fact]
    public void Can_Edit_Should_Return_True_For_Manager()
    {
        // Arrange
        var user = CreateUserWithRoles("Manager");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Edit);

        // Assert
        Assert.True(canEdit);
    }

    [Fact]
    public void Can_Edit_Should_Return_True_For_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Edit);

        // Assert
        Assert.True(canEdit);
    }

    [Fact]
    public void Can_Edit_Should_Return_False_For_Non_Manager_Or_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("User");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Edit);

        // Assert
        Assert.False(canEdit);
    }

    [Fact]
    public void Can_Delete_Should_Return_True_For_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");

        // Act
        var canDelete = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Delete);

        // Assert
        Assert.True(canDelete);
    }


    [Fact]
    public void Can_Delete_Should_Return_False_For_Non_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("Manager");

        // Act
        var canDelete = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Delete);

        // Assert
        Assert.False(canDelete);
    }

    [Fact]
    public void Can_With_Unrecognized_Action_Should_Throw_NotRecognizedActionException()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");
        var unrecognizedAction = "UnknownAction";

        // Act & Assert
        Assert.Throws<NotRecognizedActionException>(() => _authorization.Can(user, unrecognizedAction));
    }

    [Fact]
    public void Can_StartTraining_With_InstanceAction_Should_Return_True_For_Coach()
    {
        // Arrange
        var user = CreateUserWithNameAndRoles("Luigi");

        // Act
        var canStartMaintenance = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.StartTraining);

        // Assert
        Assert.True(canStartMaintenance);
    }

    [Fact]
    public void Can_Edit_Should_Return_True_For_User_With_Multiple_Roles()
    {
        // Arrange
        var user = CreateUserWithRoles("User", "Manager");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Edit);

        // Assert
        Assert.True(canEdit);
    }

    [Fact]
    public void Can_Insert_Should_Return_False_For_User_With_No_Roles()
    {
        // Arrange
        var user = CreateUserWithRoles(); // No roles

        // Act
        var canInsert = _authorization.Can(user, TeamAuthorization.Actions.Insert);

        // Assert
        Assert.False(canInsert);
    }

    [Fact]
    public void Can_StartTraining_Should_Return_False_For_Non_Coach()
    {
        // Arrange
        var user = CreateUserWithNameAndRoles("Mario");

        // Act
        var canStartTraining = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.StartTraining);

        // Assert
        Assert.False(canStartTraining);
    }

    [Fact]
    public void Can_InstanceAction_Without_Resource_Should_Throw_InstanceActionException()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");

        // Act & Assert
        Assert.Throws<InstanceActionException>(() => _authorization.Can(user, TeamAuthorization.Actions.Edit));
    }

    [Fact]
    public void Can_GenericAction_With_Resource_Should_Return_Correct_Result()
    {
        // Arrange
        var user = CreateUserWithRoles();

        // Act
        var canRead = _authorization.Can(user, _sampleResource, TeamAuthorization.Actions.Read);

        // Assert
        Assert.True(canRead);
    }

    [Fact]
    public void Can_With_Null_User_Should_Throw_ArgumentNullException()
    {
        // Arrange
        ClaimsPrincipal? user = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _authorization.Can(user, TeamAuthorization.Actions.Read));
    }

    [Fact]
    public void Can_With_Null_Action_Should_Throw_ArgumentException()
    {
        // Arrange
        var user = CreateUserWithRoles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _authorization.Can(user, null));
    }

    [Fact]
    public void Can_InstanceAction_With_Null_Resource_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");
        Team? resource = null;

        // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Throws<ArgumentNullException>(() => _authorization.Can(user, resource, TeamAuthorization.Actions.Edit));
#pragma warning restore CS8604 // Possible null reference argument.
    }

}
