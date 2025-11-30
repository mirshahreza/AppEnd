# API Development

This section explains how to build, extend, and consume AppEnd APIs.

## Generated APIs
- CRUD endpoints are generated for entities based on your database schema and module configuration
- Endpoints follow REST conventions for list, get, create, update, delete
- Responses are standardized for consistency

## Custom endpoints
- Add application services and controllers in `AppEndServer` and `AppEndHost`
- Register dependencies via DI in the host startup
- Apply validation and authorization attributes or policies

## Business rules
- Inject pre/post hooks for CRUD lifecycle events (e.g., before create, after update)
- Centralize domain logic in service classes

## Security
- Authentication: support for local or external providers (OpenID Connect planned)
- Authorization: role? and permission?based checks at controller and service levels

## Versioning and contracts
- Use DTOs and mapping to isolate internal models
- Version endpoints when introducing breaking changes

## Consuming APIs
- Directly call endpoints from your SPA
- Provide API keys or tokens where required
- Use pagination, filtering, and sorting parameters where available

See [[Security]] and [[Extensibility]].