﻿namespace AudrisAuth.Core.Tests.DefaultAuthorization;
public class TeamAuthorization : DefaultAuthorization<Team>
{
    public TeamAuthorization()
    {
        
        AddGenericRule(Actions.Read, "true");
        AddGenericRule(Actions.Insert, "HasRole(\"Manager\")");

        AddInstanceRule(Actions.Edit, "HasRole(\"Manager\") || HasRole(\"Admin\")");
        AddInstanceRule(Actions.Delete, "HasRole(\"Admin\")");
        AddInstanceRule(Actions.StartTraining, "Resource.Coach.Name == UserId");
    }

    

    public static class Actions
    {
        public static readonly string Read = nameof(Read);

        public static readonly string Insert = nameof(Insert);

        public static readonly string Edit = nameof(Edit);

        public static readonly string Delete = nameof(Delete);

        public static readonly string StartTraining = nameof(StartTraining);

    }
}