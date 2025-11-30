# Modules

Modules are feature packages that encapsulate data models, APIs, and UI components for specific business domains.

## Concepts
- Entities: data structures mapped to database tables or views
- CRUD: generated endpoints and UI for create/read/update/delete
- Lists: responsive, searchable data tables
- Forms: create/update forms with validation
- Permissions: role?based access control at module, entity, and action levels

## Creating a module
1. Define your database schema (tables, views, keys)
2. Use the builder UI to inspect the database and scaffold entities
3. Configure CRUD behavior (fields, validation rules, relations)
4. Set permissions by role/user
5. Generate the module to produce APIs and UI screens

## Customization
- Inject server?side logic: validation, mapping, business rules
- Customize UI widgets: inputs, layouts, conditional visibility
- Add search bars and filters for lists

## Packaging
- Export modules as portable packages
- Import packages to other environments
- Version and track changes

See also [[API Development]] and [[UI Development]].