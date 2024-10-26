using System.Linq.Expressions;
using System.Security.Claims;

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
            { Actions.Delete.Name, Actions.Delete },
            { Actions.StartMaintenance.Name, Actions.StartMaintenance }
        };
        
    }

    protected override void Initialize()
    {
        _genericActions.Add(Actions.Read.Name, _ruleParser.ParseGenericRule("true"));
        _genericActions.Add(Actions.Insert.Name, _ruleParser.ParseGenericRule("IsInRole(user, \"Manager\")"));

        _instanceActions.Add(Actions.Edit.Name, _ruleParser.ParseInstanceRule("IsInRole(user, \"Manager\") || IsInRole(user, \"Admin\")"));
        _instanceActions.Add(Actions.Delete.Name, _ruleParser.ParseInstanceRule("IsInRole(user, \"Admin\")"));
        _instanceActions.Add(Actions.StartMaintenance.Name, _ruleParser.ParseInstanceRule("item.IsMantainer(user.Identity.Name)"));
    }

    public static class Actions
    {
        public static readonly AuthorizationAction Read = new(nameof(Read));

        public static readonly AuthorizationAction Insert = new(nameof(Insert));

        public static readonly AuthorizationAction Edit = new(nameof(Edit), isInstanceAction: true);

        public static readonly AuthorizationAction Delete = new(nameof(Delete), isInstanceAction: true);

        public static readonly AuthorizationAction StartMaintenance = new(nameof(StartMaintenance), isInstanceAction: true);
    }
}
