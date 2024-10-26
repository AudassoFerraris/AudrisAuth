namespace AudrisAuth.Core.Tests.DefaultAuthorization;
public class SampleClassAuthorization : DefaultAuthorization<SampleClass>
{
    public SampleClassAuthorization()
    {
        _availableActions = new Dictionary<string, AuthorizationAction>
        {
            { Actions.Read.Name, Actions.Read },
            { Actions.Insert.Name, Actions.Insert },
            { Actions.Edit.Name, Actions.Edit },
            { Actions.Delete.Name, Actions.Delete }
        };
        _genericActions = new Dictionary<string, GenericAuthorizationRule>
        {
            { Actions.Read.Name, new GenericAuthorizationRule(user => true) },
            { Actions.Insert.Name, new GenericAuthorizationRule(user => user.IsInRole("Manager")) }
        };
        _instanceActions = new Dictionary<string, InstanceAuthorizationRule<SampleClass>>
        {
            { Actions.Edit.Name, new InstanceAuthorizationRule<SampleClass>((user, resource) => user.IsInRole("Manager") || user.IsInRole("Admin")) },
            { Actions.Delete.Name, new InstanceAuthorizationRule<SampleClass>((user, resource) => user.IsInRole("Admin")) }
        };
    }

    public static class Actions
    {
        public static readonly AuthorizationAction Read = new(nameof(Read));

        public static readonly AuthorizationAction Insert = new(nameof(Insert));

        public static readonly AuthorizationAction Edit = new(nameof(Edit), isInstanceAction: true);

        public static readonly AuthorizationAction Delete = new(nameof(Delete), isInstanceAction: true);
    }
}
