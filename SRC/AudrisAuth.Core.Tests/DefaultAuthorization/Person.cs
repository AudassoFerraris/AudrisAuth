namespace AudrisAuth.Core.Tests.DefaultAuthorization;

public class Person
{
    public string Name { get; set; } = null!;

    public Person(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        Name = name;
    }
}
