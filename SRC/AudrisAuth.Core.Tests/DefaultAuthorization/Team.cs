namespace AudrisAuth.Core.Tests.DefaultAuthorization;

/// <summary>
/// Class used for testing purposes as the reference type for the authorization
/// </summary>
public class Team
{
    public string Name { get; set; } = null!;

    public Person Coach { get; set; } = null!;

    public Team(string name, Person coach)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (coach == null)
        {
            throw new ArgumentNullException(nameof(coach));
        }
        Name = name;
        Coach = coach;
    }
}
