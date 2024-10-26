using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static AuthorizationAction Read = new AuthorizationAction(nameof(Read));

        public static AuthorizationAction Insert = new AuthorizationAction(nameof(Insert));

        public static AuthorizationAction Edit = new AuthorizationAction(nameof(Edit), isInstanceAction: true);

        public static AuthorizationAction Delete = new AuthorizationAction(nameof(Delete), isInstanceAction: true);
    }
}
