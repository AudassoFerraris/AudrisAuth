using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AudrisAuth.Core.Tests.DefaultAuthorization;
public class SampleClassAuthorizationTests
{
    private readonly SampleClassAuthorization _authorization;
    private readonly SampleClass _sampleResource;

    public SampleClassAuthorizationTests()
    {
        _authorization = new SampleClassAuthorization();
        _sampleResource = new SampleClass(); // Initialize as needed
    }

    private ClaimsPrincipal CreateUserWithRoles(params string[] roles)
    {
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private ClaimsPrincipal CreateUserWithRoles(IIdentity identity, params string[] roles)
    {
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        var claimsIdentity = new ClaimsIdentity(identity, claims);
        return new ClaimsPrincipal(claimsIdentity);
    }

    [Fact]
    public void Can_Read_Should_Return_True_For_Any_User()
    {
        // Arrange
        var user = CreateUserWithRoles(); // User with no roles

        // Act
        var canRead = _authorization.Can(user, SampleClassAuthorization.Actions.Read.Name);

        // Assert
        Assert.True(canRead);
    }

    [Fact]
    public void Can_Insert_Should_Return_True_For_Manager()
    {
        // Arrange
        var user = CreateUserWithRoles("Manager");

        // Act
        var canInsert = _authorization.Can(user, SampleClassAuthorization.Actions.Insert.Name);

        // Assert
        Assert.True(canInsert);
    }

    [Fact]
    public void Can_Insert_Should_Return_False_For_Non_Manager()
    {
        // Arrange
        var user = CreateUserWithRoles("User");

        // Act
        var canInsert = _authorization.Can(user, SampleClassAuthorization.Actions.Insert.Name);

        // Assert
        Assert.False(canInsert);
    }

    [Fact]
    public void Can_Edit_Should_Return_True_For_Manager()
    {
        // Arrange
        var user = CreateUserWithRoles("Manager");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.Edit.Name);

        // Assert
        Assert.True(canEdit);
    }

    [Fact]
    public void Can_Edit_Should_Return_True_For_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.Edit.Name);

        // Assert
        Assert.True(canEdit);
    }

    [Fact]
    public void Can_Edit_Should_Return_False_For_Non_Manager_Or_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("User");

        // Act
        var canEdit = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.Edit.Name);

        // Assert
        Assert.False(canEdit);
    }

    [Fact]
    public void Can_Delete_Should_Return_True_For_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");

        // Act
        var canDelete = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.Delete.Name);

        // Assert
        Assert.True(canDelete);
    }


    [Fact]
    public void Can_Delete_Should_Return_False_For_Non_Admin()
    {
        // Arrange
        var user = CreateUserWithRoles("Manager");

        // Act
        var canDelete = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.Delete.Name);

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
    public void Can_StartMaintenance_With_InstanceAction_Should_Return_True_For_Maintainer()
    {
        // Arrange
        var identity = new GenericIdentity("Luigi");
        var user = CreateUserWithRoles(identity);

        // Act
        var canStartMaintenance = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.StartMaintenance.Name);

        // Assert
        Assert.True(canStartMaintenance);
    }

    [Fact]
    public void Can_InstanceAction_Without_Resource_Should_Throw_InstanceActionException()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");

        // Act & Assert
        Assert.Throws<InstanceActionException>(() => _authorization.Can(user, SampleClassAuthorization.Actions.Edit.Name));
    }

    [Fact]
    public void Can_GenericAction_With_Resource_Should_Return_Correct_Result()
    {
        // Arrange
        var user = CreateUserWithRoles();

        // Act
        var canRead = _authorization.Can(user, _sampleResource, SampleClassAuthorization.Actions.Read.Name);

        // Assert
        Assert.True(canRead);
    }

    [Fact]
    public void Can_With_Null_User_Should_Throw_ArgumentNullException()
    {
        // Arrange
        ClaimsPrincipal user = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _authorization.Can(user, SampleClassAuthorization.Actions.Read.Name));
    }

    [Fact]
    public void Can_With_Null_Action_Should_Throw_ArgumentException()
    {
        // Arrange
        var user = CreateUserWithRoles();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _authorization.Can(user, null));
    }

    [Fact]
    public void Can_InstanceAction_With_Null_Resource_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var user = CreateUserWithRoles("Admin");
        SampleClass resource = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _authorization.Can(user, resource, SampleClassAuthorization.Actions.Edit.Name));
    }

}
