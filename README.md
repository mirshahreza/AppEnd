[![EN](https://img.shields.io/badge/lang-EN-red.svg)](README.md)
[![FA](https://img.shields.io/badge/lang-FA-blue.svg)](README.FA.md)
<p align="center" width="100%">
     <img src="/images/AppEnd-Logo-Full.png?raw=true" />
</p>

# What is AppEnd?

AppEnd is a full‑stack low‑code platform that helps you build APIs, user interfaces, and manage access levels. It is a Low‑Code and Rapid Application Development (RAD) environment.

## Why AppEnd?

There are many RAD tools—so why choose AppEnd?

- Open source and free
- Easy to use with a low learning curve
- Clean, simple, and modular architecture
- Runs on Linux and Windows
- More than database I/O: a platform to build anything and host full‑stack applications
- Developer‑friendly architecture based on common standards
- Easily inject custom code in both client and server components
- Fully customizable UIs and backends
- UIs built with Bootstrap and Vue.js 3 (easy to learn and use)
- UI translations enable multi‑language applications
- Easily inspect database structure and scaffold applications based on it
- Manage and consume APIs directly in other applications
- Generate initial end‑to‑end CRUD scenarios with just a few clicks
- Suitable for both back‑office and front‑office applications
- Built‑in module to manage deployment tasks
- Supports single‑ or multi‑node deployments
- Actively developed

## Technology

- Host: Linux or Windows
- Application Server: .NET (C#)
- Database: Microsoft SQL Server
- Client: SPA built with Bootstrap and Vue.js 3

## Roadmap

Database‑centric applications typically include the following sections, with user access levels in mind:

1. Application builder & CRUD functionalities: In progress
   - Make tables and lists responsive
   - New UI widgets to improve create/update forms
   - More advanced search bars for generated lists
   - UI designer
   - Docker image for easy installation
   - Package Manager to create/import/export packages as portable plugins
   - Git integration to manage production
   - OpenID Connect (SSO)
   - Task Scheduler

2. Workflow Engine: Planning

3. Reporting and Visualization system: Planning

We will start phase 2 and 3 after phase 1 becomes stable enough.

## Getting Started

### Run the project

1. Clone the repository.
2. Open the solution in Visual Studio 2022.
3. Set up the SQL Server database:
   1. Create an empty database in your SQL Server instance.
   2. Download the `Zzz_Deploy.sql` script from: https://github.com/mirshahreza/RDBMS-PackageManager/blob/master/MsSql/Zzz_Deploy.sql
   3. In SQL Server Management Studio (SSMS):
      - File → Open → File… and select `Zzz_Deploy.sql`, then click Execute.
      - Right‑click your newly created database and choose New Query.
   4. Run the following commands in a blank query window, then Execute. You should now see the tables and views in your database:

      ```sql
      EXEC Zzz_Deploy;
      EXEC Zzz_Deploy 'AppEnd';
      ```

4. Update the database connection string in `AppEnd/AppEndHost/appsettings.json`.
5. Run the `AppEndHost` project.
   - Default credentials: Username `Admin`, Password `P#ssw0rd`.

## Documentation

For more information, see the project wiki: https://github.com/mirshahreza/AppEnd/wiki

## Support

You can support the project by:

1. Participating in development
2. Donating
