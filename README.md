﻿# AudrisAuth

[![NuGet Version](https://img.shields.io/nuget/v/AudrisAuth.svg?style=flat)](https://www.nuget.org/packages/AudrisAuth/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/AudassoFerraris/AudrisAuth/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AudassoFerraris/AudrisAuth/actions)

An extensible and flexible authorization library for .NET applications, allowing you to define and evaluate authorization rules dynamically using expressions. AudrisAuth leverages [DynamicExpresso](https://github.com/davideicardi/DynamicExpresso) to parse and evaluate authorization rules defined as strings.

## Status

**Current Version**: `1.0.3-beta`

⚠️ This library is currently in Beta. APIs may change, and backward compatibility is not guaranteed. Feedback is welcome!

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
  - [Installation](#installation)
  - [Prerequisites](#prerequisites)
- [Usage](#usage)
  - [Defining Authorization Rules](#defining-authorization-rules)
  - [Implementing a Custom Authorization Class](#implementing-a-custom-authorization-class)
  - [Checking Permissions](#checking-permissions)
  - [Example](#example)
- [Security Considerations](#security-considerations)
- [License](#license)
- [Contributing](#contributing)
- [Acknowledgments](#acknowledgments)

## Features

- **Dynamic Rule Evaluation**: Define authorization rules as strings and evaluate them at runtime.
- **Extensibility**: Customize the authorization logic by overriding methods and adding custom variables.
- **Support for Claims and Roles**: Utilize user roles and claims in your authorization rules.
- **Instance and Generic Actions**: Define both instance-specific and generic actions.
- **Compatibility with LINQ**: Generate expressions to use in LINQ queries for efficient data filtering.


## Getting Started

### Installation

To include AudrisAuth in your project, you can install it via NuGet:

```shell
Install-Package AudrisAuth
```

Or using the .NET CLI:

```shell
dotnet add package AudrisAuth
```

### Prerequisites

- **.NET Standard 2.0** compatible framework:
  - **.NET Framework 4.6.1** or later
  - **.NET Core 2.0** or later
- [DynamicExpresso](https://github.com/davideicardi/DynamicExpresso) (installed automatically via NuGet)

## Usage

### Defining Authorization Rules

Authorization rules are defined as strings using a simple expression language. You can use variables and functions provided by the library to create dynamic and flexible rules.

**Available Variables:**

- `UserId`: The identifier of the current user (`string`).
- `UserRoles`: An array of roles assigned to the user (`string[]`).
- `UserClaims`: A dictionary of the user's claims (`Dictionary<string, List<string>>`).
- `Resource`: The resource instance being accessed (available in instance actions).

### Implementing a Custom Authorization Class

To create your custom authorization logic, derive a class from `DefaultAuthorization<T>`, where `T` is the type of the resource you are securing.

```csharp
using AudrisAuth;
using System.Security.Claims;

public class TeamAuthorization : DefaultAuthorization<Team>
{
    public TeamAuthorization()
    {
        // Define generic actions
        AddGenericRule("Read", "true");
        AddGenericRule("Insert", "UserRoles.Contains(\"Manager\")");

        // Define instance actions
        AddInstanceRule("Edit", "UserRoles.Contains(\"Manager\") || UserRoles.Contains(\"Admin\")");
        AddInstanceRule("Delete", "UserRoles.Contains(\"Admin\")");
        AddInstanceRule("StartTraining", "Resource.Coach.Name == UserId || UserClaims.Contains(\"TeamCoach:\"+Resource.Name)");
    }
}
```

### Checking Permissions

Use the `Can` methods to check if a user can perform a specific action.

```csharp
// Create an instance of your authorization class
var authorization = new TeamAuthorization();

// Create a ClaimsPrincipal for the user (this would typically come from your authentication system)
var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.NameIdentifier, "Luigi"),
    new Claim(ClaimTypes.Role, "Manager")
}, "TestAuthType"));

// Check generic action
bool canRead = authorization.Can(user, "Read");

// Check instance action
var team = new Team { Name = "Black Bugs", Coach = new Person { Name = "Luigi" } };
bool canEdit = authorization.Can(user, team, "Edit");
```

### Get an Expression to use in LINQ Queries

You can retrieve the expression for a specific action to use in LINQ queries.

```csharp
var authExpression = authorization.GetAuthorizationExpression(user, "StartTraining");

var teams = context.Teams
    .Include(t => t.Coach)
    .Where(authExpression)
    .OrderBy(t => t.Name)
    .ToList();
```

### Example

Here's a complete example demonstrating how to use AudrisAuth in an application.

**Step 1: Define Your Resource Classes**

```csharp
public class Person
{
    public string Name { get; set; }
}

public class Team
{
    public string Name { get; set; }
    public Person Coach { get; set; }
}
```

**Step 2: Implement Your Authorization Class**

```csharp
public class TeamAuthorization : DefaultAuthorization<Team>
{
    public TeamAuthorization()
    {
        // Define generic actions
        AddGenericRule("Read", "true");
        AddGenericRule("Insert", "UserRoles.Contains(\"Manager\")");

        // Define instance actions
        AddInstanceRule("Edit", "UserRoles.Contains(\"Manager\") || UserRoles.Contains(\"Admin\")");
        AddInstanceRule("Delete", "UserRoles.Contains(\"Admin\")");
        AddInstanceRule("StartTraining", "Resource.Coach.Name == UserId || UserClaims.Contains(\"TeamCoach:\"+Resource.Name)");
    }
}
```

**Step 3: Use the Authorization Class in Your Code**

```csharp
// Initialize the authorization class
var authorization = new TeamAuthorization();

// Simulate an authenticated user
var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.NameIdentifier, "Luigi"),
    new Claim(ClaimTypes.Role, "Manager")
}, "TestAuthType"));

// Create a team resource
var team = new Team
{
    Name = "Black Bugs",
    Coach = new Person { Name = "Luigi" }
};

// Check permissions
if (authorization.Can(user, "Insert"))
{
    // User can insert teams
}

if (authorization.Can(user, team, "Edit"))
{
    // User can edit the team
}

if (authorization.Can(user, team, "StartTraining"))
{
    // User can start training for the team
}
```

### Extending the Interpreter

You can extend the interpreter by overriding the `ConfigureInterpreter` method to add variables.

```csharp
protected override void ConfigureInterpreter(Interpreter interpreter)
{
    base.ConfigureInterpreter(interpreter);

    // Add a custom function
    interpreter.SetVariable("Context", myContextData);
}
```

## Testing Authorization Rules

It's important to test your authorization rules to ensure they behave as expected.

**Example using xUnit:**

```csharp
public class TeamAuthorizationTests
{
    [Fact]
    public void Manager_Can_Insert_Teams()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "Alice"),
            new Claim(ClaimTypes.Role, "Manager")
        }, "TestAuthType"));

        var authorization = new TeamAuthorization();

        // Act
        bool canInsert = authorization.Can(user, "Insert");

        // Assert
        Assert.True(canInsert);
    }
}
```

## Security Considerations

- **Expression Security**: By default, the interpreter restricts accessible types to those explicitly referenced. Ensure that only necessary types are referenced to prevent unauthorized access.
- **Reflection Disabled**: Reflection is disabled by default in DynamicExpresso, enhancing security.
- **Input Validation**: If rules are sourced from external inputs, validate and sanitize them to prevent injection attacks.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please open issues or submit pull requests for any changes.

1. Fork the repository.
2. Create your feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -am 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

## Support

If you have any questions or need assistance, please open an issue on [GitHub](https://github.com/AudassoFerraris/AudrisAuth/issues).


## Acknowledgments

- [DynamicExpresso](https://github.com/davideicardi/DynamicExpresso) - For the expression parsing and evaluation engine.
- Crafted in collaboration with [ChatGPT](https://openai.com/blog/chatgpt) by OpenAI.
- Inspired by discussions and feedback from the developer community.

