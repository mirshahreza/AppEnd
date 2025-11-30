# Contributing to AppEnd

Thanks for your interest in contributing! This guide explains how to propose changes and get them merged smoothly.

Note: Please use English for issues, discussions, commit messages, and pull requests.

## Ways to contribute
- Report bugs
- Propose features or improvements
- Improve documentation
- Refactor for clarity/maintainability
- Performance optimizations

## Before you start
- Search existing issues and PRs to avoid duplicates
- For significant changes, open an issue first to discuss the approach
- Keep PRs focused and small where possible

## Development setup
- Requirements:
  - Windows or Linux
  - .NET SDK (latest supported by the repo)
  - Visual Studio 2022 (or newer) or VS Code with C# extension
  - SQL Server instance and SSMS (optional but recommended)
  - Git
- Clone the repository
- Database:
  - Follow the steps in `README.md` (Getting Started) to create the database and run `Zzz_Deploy`
  - Update connection string in `AppEnd/AppEndHost/appsettings.json`
- Build and run:
  - `dotnet restore`
  - `dotnet build`
  - `dotnet run --project AppEnd/AppEndHost/AppEndHost.csproj`

## Branching model
- Default branch: `master`
- Create topic branches from `master`:
  - `feature/<short-name>` for features
  - `fix/<short-name>` for bug fixes
  - `docs/<short-name>` for documentation
- Rebase (preferred) or merge `master` into your branch to stay up to date

## Commit messages
Use Conventional Commits:
- Types: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `ci`, `chore`, `perf`
- Optional scope: `feat(AppEndHost): ...`, `fix(AppEndServer): ...`
- Example:
  - `feat(AppEndHost): add JWT authentication settings validation`

## Coding guidelines
- Match the existing code style and conventions in the repository
- Prefer readability and small, composable methods
- Use dependency injection where applicable
- Handle exceptions thoughtfully; avoid swallowing errors silently
- Add XML docs or comments where the intent is not obvious
- Avoid introducing new warnings; build should be clean

## Tests
- Add unit/integration tests when applicable for new behavior or bug fixes
- Ensure all tests pass locally before submitting a PR
- If no test project exists for your change, consider adding one or justify in the PR why tests are not included

## Documentation
- Update `README.md` or the wiki when you change behavior, configuration, or setup steps
- Keep examples runnable and accurate

## Pull request checklist
- The PR title follows Conventional Commits
- Linked issue (if applicable)
- Clear description of the change and rationale
- Steps to test or reproduce
- No secrets or credentials committed
- Build passes locally (and CI if configured)
- Database migrations/scripts updated if required

## Database changes
- Include scripts needed to initialize or migrate the database
- Keep changes backwards compatible when possible; document breaking changes clearly

## Security
- Do not disclose vulnerabilities in public issues
- Use GitHub Security Advisories (if enabled) or contact the maintainers privately
- Provide clear reproduction steps and potential impact

## Licensing
By contributing, you agree that your contributions will be licensed under the repository’s license.

## Questions
Open a discussion or issue if you are unsure about anything before you start. We are happy to help.
