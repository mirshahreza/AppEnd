# Database

AppEnd is database?centric and uses SQL Server.

## Initialization
- Use `Zzz_Deploy.sql` to bootstrap the platform database objects
- Run `EXEC Zzz_Deploy;` and `EXEC Zzz_Deploy 'AppEnd';`

## Modeling
- Design normalized tables with keys and constraints
- Views can be used for read?only entities
- Use naming consistency to simplify scaffolding

## Migrations
- Include scripts for schema changes with each module update
- Document breaking changes clearly in PRs and wiki

## Performance
- Add indexes to support frequent queries
- Avoid N+1 patterns by shaping queries appropriately

## Security
- Principle of least privilege for DB users
- Avoid dynamic SQL where possible; parameterize queries

See [[Modules]] and [[API Development]].