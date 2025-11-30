# Getting Started

This guide helps you set up AppEnd locally and explore its core features.

## Prerequisites
- Windows or Linux
- .NET SDK (latest supported, e.g., .NET 10)
- Visual Studio 2022+ or VS Code with C# extension
- SQL Server instance and SSMS (or Azure Data Studio)
- Git

## Clone and open
1. `git clone https://github.com/mirshahreza/AppEnd.git`
2. Open the solution in Visual Studio 2022

## Database setup
1. Create an empty database in your SQL Server instance
2. Download `Zzz_Deploy.sql` from:
   https://github.com/mirshahreza/RDBMS-PackageManager/blob/master/MsSql/Zzz_Deploy.sql
3. In SSMS:
   - File ? Open ? File… select `Zzz_Deploy.sql`, Execute
   - Right?click your new database ? New Query
4. Run:
   ```sql
   EXEC Zzz_Deploy;
   EXEC Zzz_Deploy 'AppEnd';
   ```

## Configure app
- Edit `AppEnd/AppEndHost/appsettings.json` and set your connection string

## Run
- Start `AppEndHost` project
- Default credentials:
  - Username: `Admin`
  - Password: `P#ssw0rd`

## Next steps
- Learn about [[Architecture]]
- Build your first CRUD with [[Modules]] and [[API Development]]
- Customize UI with [[UI Development]]
- See [[Deployment]] to publish