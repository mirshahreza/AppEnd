# Security

Security spans application, API, and database layers.

## Authentication
- Local authentication supported; OpenID Connect SSO planned
- Store secrets securely and rotate regularly

## Authorization
- Role? and permission?based access controls
- Enforce checks at API, service, and UI levels

## Data protection
- HTTPS only in production
- Avoid logging sensitive data
- Encrypt at rest where hosted platform supports it

## Secure coding
- Validate and sanitize inputs
- Use parameterized queries
- Handle exceptions without leaking sensitive details

## Operational security
- Patch dependencies regularly
- Principle of least privilege for services and DB users

See [[Configuration]] and [[Deployment]].