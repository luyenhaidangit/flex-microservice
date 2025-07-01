# Contributing to Flex Microservice

We welcome contributions to improve the project. Before submitting a pull request, please follow these guidelines.

## Commit Message Style

Use the format **`type(scope): short message`** for all commit messages.
Examples:

```
feat(identity): add role approval flow
fix(ordering): handle null order date
```

The `type` should be `feat`, `fix`, `docs`, `refactor`, or another descriptive tag. The `scope` is the component or module being changed.

## Development Checklist

Run the following commands before opening a pull request to ensure consistency:

```bash
dotnet format
dotnet build
dotnet test
```

## Pull Requests

- Keep changes focused and provide a clear description.
- Reference any related issues.
- Ensure your commit messages follow the style noted above.

Thank you for contributing!
