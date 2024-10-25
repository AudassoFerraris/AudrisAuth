# AudrisAuth

**AudrisAuth** is a .NET Standard 2.0 library for permission management based on the **RBAC** (Role-Based Access Control) model. Designed to be flexible and adaptable, **AudrisAuth** allows you to define and verify user permissions on specific objects, supporting both general authentication scenarios and contextual authorization checks.

## Features

- **Role-Based Control**: Define permissions based on user roles with fluent authorization handling.
- **Flexible Contextual Authorization**: Verify permissions for specific instances, enabling contextual scenarios like checking maintenance permissions on specific machinery or equipment.
- **Adaptable to Legacy Systems**: Using `ClaimsPrincipal` and an adapter system, the library can be easily integrated even in applications with legacy user management or systems not natively designed for RBAC.

## Requirements

- **.NET Standard 2.0**: Compatible with a wide range of .NET applications, including projects based on .NET Framework and .NET Core.
- **ClaimsPrincipal**: Uses `ClaimsPrincipal` for managing user identities, allowing seamless integration with existing authentication systems.

## Getting Started

1. **Installation**: Add **AudrisAuth** to your project via NuGet.
2. **Role and Permission Configuration**: Set up the roles and permissions required for your application.
3. **Fluent Methods Usage**: Verify permissions using fluent syntax, making the code readable and intuitive.

## Usage Examples

**AudrisAuth** makes permission management straightforward and direct. Here are some common scenarios:
- **Generic Verification**: Check user permissions for general actions like `List` or `Create`.
- **Contextual Verification**: Evaluate permissions on specific instances, such as verifying if a user can perform a particular action on a given resource.
- **Custom Roles**: Integrate custom roles and permissions verification based on application-defined contexts.

## Contributing

We welcome contributions to improve **AudrisAuth**! Feel free to open issues, propose pull requests, or suggest new features.

## License

**AudrisAuth** is distributed under the MIT License. See the `LICENSE` file for more details.
