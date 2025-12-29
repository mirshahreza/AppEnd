<!--
Document Direction: Left-to-Right (LTR)
Language: English
-->

<div dir="ltr" lang="en">

# Complete AppEnd Framework Architecture and Workflow Documentation

## Table of Contents
1. [Introduction](#introduction)
2. [AppEnd Overview](#append-overview)
3. [Overall System Architecture](#overall-system-architecture)
4. [Project Modules](#project-modules)
5. [Workflow](#workflow)
6. [Key Features](#key-features)
7. [How to Create an Application](#how-to-create-an-application)
8. [Development Guide](#development-guide)
9. [Debugging and Changes](#debugging-and-changes)
10. [File and Folder Structure](#file-and-folder-structure)
11. [Complete Configuration Reference](#complete-configuration-reference)
12. [UI Widgets and Components](#ui-widgets-and-components)
13. [Compare Operators](#compare-operators)
14. [Relations (Table Relationships)](#relations-table-relationships)
15. [ValueSharp System](#valuesharp-system)
16. [Package Manager](#package-manager)
17. [Multiple Database Support](#multiple-database-support)
18. [RPC Client Library (append-client.js)](#rpc-client-library-append-clientjs)
19. [Frontend Application Structure](#frontend-application-structure)
20. [BuildInfo and Template System](#buildinfo-and-template-system)
21. [Database Schema Utilities](#database-schema-utilities)
22. [Security and Best Practices](#security-and-best-practices)
23. [Pagination and Sorting](#pagination-and-sorting)
24. [DbQueryColumn and Column Selection](#dbquerycolumn-and-column-selection)
25. [Aggregations](#aggregations)
26. [History Tables](#history-tables)
27. [ClientUI Structure](#clientui-structure)
28. [FAQ (Frequently Asked Questions)](#faq-frequently-asked-questions)
29. [Practical Examples](#practical-examples)
30. [Glossary](#glossary)
31. [Frontend Architecture](#frontend-architecture)

---

## Introduction

AppEnd is a Low-Code and Rapid Application Development (RAD) framework that helps you quickly build complete Full-Stack applications. This framework is designed with a simple and modular architecture with high development and customization capabilities.

---

## AppEnd Overview

### What is AppEnd?

AppEnd is a Low-Code platform that enables rapid creation of APIs, user interfaces, and access level management. This framework is designed for building database-driven applications.

### Benefits of Using AppEnd

- **Open Source and Free**: Completely open source
- **Easy to Learn**: Low learning curve
- **Clean and Simple Architecture**: Modular and understandable structure
- **Runs on Linux and Windows**
- **Not Just for Database IO**: A complete platform for application development
- **Compatible with Development Standards**
- **Custom Code Injection Capability**: In both client and server sections
- **Customizable User Interfaces**
- **Multi-language Support**
- **Automatic CRUD Generation**: With a few simple clicks

### Technologies Used

- **Host**: Linux or Windows
- **Application Server**: .NET Core / C#
- **Database**: MS SQL Server
- **Client**: SPA based on Bootstrap & Vue.js 3

---

## Overall System Architecture

### General Architecture

AppEnd uses a three-layer architecture:

```
┌─────────────────────────────────────────┐
│         Frontend (Vue.js + Bootstrap)   │
│  - SPA Application                      │
│  - Components & Templates               │
└─────────────────┬───────────────────────┘
                  │ RPC (JSON over HTTP)
┌─────────────────▼───────────────────────┐
│         AppEndHost (ASP.NET Core)       │
│  - HTTP Server                          │
│  - RPC Endpoint Handler                 │
│  - Static File Server                   │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      AppEndServer (Business Logic)      │
│  - RpcNet (RPC Handler)                 │
│  - Dynamic Code Execution               │
│  - Service Layer                        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│     AppEndDbIO (Data Access Layer)      │
│  - Database Connection                  │
│  - Query Builder                        │
│  - CRUD Operations                      │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         SQL Server Database             │
└─────────────────────────────────────────┘
```

### Communication Between Layers

#### 1. Frontend to Backend Communication

Frontend communicates with Backend via **RPC (Remote Procedure Call)**:

- **Endpoint**: `/talk-to-me` (configurable in appsettings.json)
- **Method**: POST
- **Format**: JSON
- **Authentication**: Token-based (in Header)

#### 2. Request Processing

1. Frontend sends RPC request
2. `RpcNet` receives the request
3. `DynaCode` finds and executes the corresponding method
4. Method can use `DbIO` to access database
5. Result is returned to Frontend

---

## Project Modules

The AppEnd project is divided into several modules:

### 1. AppEndCommon

**Responsibility**: Common classes, functions, and utility tools

**Important Files**:
- `AppEndClass.cs`: Base classes for C# code generation
- `AppEndSettings.cs`: System settings
- `AppEndUser.cs`: User model
- `AppEndException.cs`: Custom Exception class
- `ExtensionsFor*.cs`: Extension Methods for various types

**Key Features**:
- Extension Methods for String, DateTime, Json, etc.
- Settings management through `appsettings.json`
- Helper classes for logging and error management

### 2. AppEndDbIO

**Responsibility**: Database access and query building

**Main Classes**:
- `DbIO.cs`: Base class for database communication
  - `DbIOMsSql.cs`: Implementation for SQL Server
- `ClientQuery.cs`: Building and executing queries from JSON
- `DbQuery.cs`: Defining predefined queries
- `DbDialog.cs`: Defining metadata for a table/view
- `DbDialogFactory.cs`: Factory for creating DbDialog

**QueryType Types**:
- `Create`: For Insert
- `ReadList`: For SELECT list
- `ReadByKey`: For SELECT single record
- `UpdateByKey`: For Update
- `DeleteByKey`: For Delete
- `AggregatedReadList`: For SELECT with Aggregate Functions
- `Procedure`: For executing Stored Procedure
- `ScalarFunction`: For executing Scalar Function
- `TableFunction`: For executing Table Function

**Usage Example**:
```csharp
// Create a ClientQuery from JSON
ClientQuery cq = ClientQuery.GetInstanceByQueryJson(jsonElement, userContext);
object result = cq.Exec(); // Execute query
```

### 3. AppEndDynaCode

**Responsibility**: Dynamic C# code compilation and execution

**Main Class**: `DynaCode.cs`

**Features**:
- Compile `.cs` files in `workspace/server`
- Create dynamic assembly
- Dynamically invoke methods
- Access control management
- Result caching
- Automatic logging

**Workflow**:
1. All `.cs` files in `workspace/server` folder are read
2. Compiled using Roslyn
3. Dynamic assembly is created
4. Methods are called via Reflection

**Example**:
```csharp
// Invoke a method
CodeInvokeResult result = DynaCode.InvokeByJsonInputs(
    "DefaultRepo.Products.ReadList",
    jsonInputs,
    actor
);
```

### 4. AppEndServer

**Responsibility**: Server services and Business Logic

**Main Classes**:
- `RpcNet.cs`: Middleware for RPC management
- `DbServices.cs`: Database-related services
- `DbDialogServices.cs`: DbDialog-related services
- `TemplateServices.cs`: UI generation service from Template
- `ActorServices.cs`: Authentication services
- `CacheServices.cs`: Cache management
- `FileServices.cs`: File management

**Features**:
- RPC Request management
- Authentication & Authorization
- Automatic UI generation from Templates
- Cache management
- Logging

### 5. AppEndHost

**Responsibility**: Application hosting and HTTP Server

**Important Files**:
- `Program.cs`: Application entry point

**Responsibilities**:
- Start HTTP Server
- Configure Middleware
- Serve static files (Frontend)
- Configure CORS
- Response Compression

### 6. Frontend (workspace/client)

**Responsibility**: User Interface

**Structure**:
- `a..lib/`: JavaScript libraries
  - `append-client.js`: Main RPC client
  - `jquery/`, `bootstrap/`, `vue/`: Required libraries
- `a..templates/`: Razor templates for UI generation
- `a.Components/`: Generated Vue components
- `a.SharedComponents/`: Shared Vue components
- `a.Layouts/`: Vue layouts
- `AppEndStudio/`: Studio application for management
- `[ApplicationName]/`: Created applications

---

## Workflow

### 1. Flow of a Request from Frontend to Database

```
1. User performs an Action in Frontend
   ↓
2. Frontend creates an RPC Request
   {
     "Id": "unique-id",
     "Method": "DefaultRepo.Products.ReadList",
     "Inputs": { ... }
   }
   ↓
3. Request is sent to /talk-to-me
   ↓
4. RpcNet receives the request
   ↓
5. Actor (user) is extracted from Token
   ↓
6. DynaCode finds the method and checks access
   ↓
7. Method is executed (may use Cache)
   ↓
8. Method may create and execute ClientQuery
   ↓
9. DbIO executes SQL query
   ↓
10. Result is returned from Database
   ↓
11. Result is processed and converted to JSON
   ↓
12. Response is returned to Frontend
   ↓
13. Frontend updates UI
```

### 2. Creating a New DbObject (Table) Flow

```
1. Developer creates a new table in Database
   ↓
2. In AppEndStudio, select the table
   ↓
3. Click on "Create Server Objects"
   ↓
4. DbDialogFactory.CreateServerObjectsFor() is called
   ↓
5. DbDialog JSON is created (workspace/server/DefaultRepo.TableName.dbdialog.json)
   ↓
6. C# class is created (workspace/server/DefaultRepo.TableName.cs)
   ↓
7. Default queries are created (Create, ReadList, ReadByKey, UpdateByKey, DeleteByKey)
   ↓
8. ClientUIs are defined
   ↓
9. DynaCode.Refresh() is called (recompilation)
   ↓
10. UI can be generated from Template
```

### 3. UI Generation from Template Flow

```
1. Developer clicks on "Build UI"
   ↓
2. DbDialogServices.BuildUiForDbObject() is called
   ↓
3. For each ClientUI:
   - Corresponding template is read (a..templates/TemplateName.cshtml)
   - Compiled using RazorEngine
   - BuildInfo is passed to Template
   ↓
4. Vue Component is created (.vue)
   ↓
5. File is saved in a.Components/
```

---

## Key Features

### 1. Authentication & Authorization

**Authentication**:
- Token-based Authentication
- Token is sent in Header with name `token`
- Token contains user information (Id, UserName, Roles)
- Token is encoded/decoded with Secret Key

**Authorization**:
- Method level: Each method can have Access Rules
- Role level: Access based on Role
- User level: Access based on UserName
- Public Methods: Public methods that don't require authentication

**Access Settings Example**:
```json
{
  "AccessRules": {
    "AllowedRoles": ["admin", "user"],
    "AllowedUsers": ["john"],
    "DeniedUsers": []
  }
}
```

### 2. Dynamic Code Compilation

- C# code in `workspace/server` is dynamically compiled
- Uses Roslyn for compilation
- Creates dynamic assembly
- Can Refresh without restarting application

### 3. Query Builder

- Build SQL query from JSON
- Supports:
  - WHERE clauses (with various CompareOperators)
  - ORDER BY
  - Pagination
  - Aggregations
  - Relations (JOIN)
  - Sub Queries

### 4. Template Engine

- Uses RazorEngine for UI generation
- Razor templates in `a..templates/`
- Automatic Vue Components generation
- Available templates:
  - `ReadList.cshtml`: Data list
  - `Create.cshtml`: Create form
  - `UpdateByKey.cshtml`: Edit form
  - `ReadByKey.cshtml`: Display single record

### 5. Caching

- Memory Cache for method results
- Configurable Cache Policy:
  - `None`: No caching
  - `Global`: Cache for everyone
  - `PerUser`: Cache per user
- Configurable TTL

### 6. Logging

- Automatic logging for all methods
- Save to Database (BaseActivityLog)
- Includes information:
  - Method Name
  - Inputs
  - Output
  - Duration
  - User
  - IP Address
  - Success/Failure

### 7. File Management

- Static file management
- Upload/Download support
- Image and binary file management

---

## How to Create an Application

### Step 1: Initial Setup

1. **Database Setup**:
   - Create an empty Database in SQL Server
   - Run `Zzz_Deploy.sql` file
   - Execute the following commands:
   ```sql
   EXEC Zzz_Deploy
   EXEC Zzz_Deploy 'AppEnd'
   ```

2. **appsettings.json Configuration**:
   ```json
   {
     "AppEnd": {
       "DefaultDbConfName": "DefaultRepo",
       "TalkPoint": "talk-to-me",
       "Secret": "YourSecretKey",
       "DbServers": [
         {
           "Name": "DefaultRepo",
           "ServerType": "MsSql",
           "ConnectionString": "YourConnectionString"
         }
       ]
     }
   }
   ```

3. **Run Application**:
   - Run `AppEndHost` project
   - Navigate to `http://localhost:5000`
   - Login with Username: `Admin` and Password: `P#ssw0rd`

### Step 2: Create a New Table

1. **Create Table in Database**:
   ```sql
   CREATE TABLE Products (
       Id INT PRIMARY KEY IDENTITY(1,1),
       Name NVARCHAR(200) NOT NULL,
       Price DECIMAL(18,2),
       CreatedOn DATETIME DEFAULT GETDATE()
   )
   ```

2. **Create Server Objects**:
   - Enter AppEndStudio
   - Go to "DbObjects" section
   - Find the `Products` table
   - Click on "Create Server Objects"
   - This creates:
     - `DefaultRepo.Products.dbdialog.json`
     - `DefaultRepo.Products.cs`

3. **Configure DbDialog**:
   - Open `DefaultRepo.Products.dbdialog.json` file
   - You can configure Column Properties, Relations, and UI Props

### Step 3: Create Queries

1. **Create Default Queries**:
   - In Designer, you can create various queries:
     - `ReadList`: For displaying list
     - `Create`: For creating new record
     - `ReadByKey`: For displaying one record
     - `UpdateByKey`: For editing
     - `DeleteByKey`: For deletion

2. **Customize Query**:
   - Select Columns
   - Configure Relations
   - Add Aggregations
   - Configure WHERE clauses

### Step 4: Generate UI

1. **Create ClientUI**:
   - For each Query that needs UI, create a ClientUI
   - Select Template (ReadList, Create, UpdateByKey, ...)

2. **Build UI**:
   - Click on "Build UI"
   - Vue Components are created in `a.Components/`

### Step 5: Create Frontend Application

1. **Create Application Folder**:
   - Create a new folder in `workspace/client/` (e.g., `MyApp`)

2. **Create app.json**:
   ```json
   {
     "title": "My App",
     "sub-title": "Application",
     "dir": "ltr",
     "lang": "en",
     "defaultComponent": "components/Home",
     "navigation": [
       {
         "title": "Products",
         "icon": "fa-box",
         "component": "a.Components/Products_ReadList"
       }
     ]
   }
   ```

3. **Access Application**:
   - Navigate to `http://localhost:5000/client/MyApp/index.html`

---

## Development Guide

### 1. Creating a New Method (Not Mapped)

Methods that are not directly related to Database:

```csharp
// File: workspace/server/MyNamespace.MyClass.cs
namespace MyNamespace
{
    public static class MyClass
    {
        public static object? MyCustomMethod(AppEndUser? Actor, string Parameter1)
        {
            // Your code
            return new { Result = "Success" };
        }
    }
}
```

**Access Settings** (file `.settings.json`):
```json
{
  "MyNamespace.MyClass.MyCustomMethod": {
    "AccessRules": {
      "AllowedRoles": ["admin"],
      "AllowedUsers": []
    },
    "CachePolicy": {
      "CacheLevel": "None"
    },
    "LogPolicy": "Full"
  }
}
```

### 2. Creating a DbDialog Method

Methods that use DbDialog:

```csharp
namespace DefaultRepo
{
    public static class Products
    {
        public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
    }
}
```

### 3. Customizing a Method

You can edit the generated methods:

```csharp
public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
{
    // Before executing Query
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo);
    
    // Add WHERE clause
    if (cq.Where == null) cq.Where = new Where();
    cq.Where.And("IsActive", CompareOperator.Equal, true);
    
    // Execute Query
    object result = cq.Exec();
    
    // Process result
    // ...
    
    return result;
}
```

### 4. Creating a Custom Vue Component

1. **Create Vue File**:
   ```vue
   <!-- workspace/client/MyApp/components/MyComponent.vue -->
   <template>
     <div>
       <h1>My Custom Component</h1>
     </div>
   </template>

   <script>
   export default {
     name: 'MyComponent',
     data() {
       return {
         // data
       }
     },
     methods: {
       // methods
     }
   }
   </script>
   ```

2. **Use in app.json**:
   ```json
   {
     "navigation": [
       {
         "title": "My Component",
         "component": "components/MyComponent"
       }
     ]
   }
   ```

### 5. Creating a New Template

1. **Create Template File**:
   ```razor
   @* workspace/client/a..templates/MyTemplate.cshtml *@
   <template>
     <div>
       <h1>@Model.DbDialog.ObjectName</h1>
       <!-- ... -->
     </div>
   </template>
   ```

2. **Use in ClientUI**:
   - In DbDialog, set ClientUI with TemplateName = "MyTemplate"

### 6. Working with Cache

```csharp
public static object? GetCachedData(AppEndUser? Actor)
{
    string cacheKey = "MyCacheKey";
    
    if (SV.SharedMemoryCache.TryGetValue(cacheKey, out object? cached))
    {
        return cached;
    }
    
    // Compute data
    object data = ComputeData();
    
    // Cache it
    MemoryCacheEntryOptions options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };
    SV.SharedMemoryCache.ToCache(cacheKey, data, options);
    
    return data;
}
```

### 7. Working with Logging

```csharp
// Log Error
LogMan.LogError("Error message");

// Log Activity (automatically done by DynaCode)
LogMan.LogActivity(
    namespaceName,
    className,
    methodName,
    recordId,
    isSucceeded,
    fromCache,
    inputs,
    response,
    duration,
    clientIp,
    clientAgent,
    userId,
    userName
);
```

---

## Debugging and Changes

### 1. Debugging Backend

**Using Visual Studio**:
1. Set Breakpoint
2. Run `AppEndHost` project in Debug mode
3. Send request from Frontend
4. Breakpoint is hit

**Logs**:
- Logs are stored in `log/`
- Also saved in Database (BaseActivityLog)

**View Exception**:
- In Log Files
- In Database Activity Log
- In Console Output

### 2. Debugging Frontend

**Using Browser DevTools**:
1. Press F12
2. Go to Console
3. View errors and logs

**View RPC Requests**:
1. Go to Network Tab
2. Find request to `/talk-to-me`
3. View Request and Response

### 3. Changes in Backend Code

1. **Changes in Dynamic Code**:
   - Edit `.cs` file in `workspace/server/`
   - DynaCode automatically detects changes
   - You can call `DynaCode.Refresh()`

2. **Changes in Framework**:
   - Edit Framework files
   - Restart application

### 4. Changes in Frontend

1. **Changes in Components**:
   - Edit `.vue` file
   - Refresh page (Hot Reload may be available)

2. **Changes in Templates**:
   - Edit Template
   - Rebuild UI

### 5. Troubleshooting

**Issue: Method not found**
- Ensure `.cs` file is in `workspace/server/`
- Check Namespace and Class Name
- Call `DynaCode.Refresh()`
- Check compile errors in Log

**Issue: Access denied**
- Check AccessRules settings
- Check user Role
- Check PublicMethods

**Issue: Query not executing**
- Check Connection String
- Check Query structure
- Check Database logs

---

## File and Folder Structure

### General Structure

```
AppEnd/
├── AppEndCommon/          # Common module
├── AppEndDbIO/            # Database IO module
├── AppEndDynaCode/        # Dynamic Code module
├── AppEndServer/          # Server Services module
├── AppEndHost/            # Host module
│   ├── workspace/
│   │   ├── server/        # Dynamic C# code
│   │   │   ├── DefaultRepo.Products.cs
│   │   │   ├── DefaultRepo.Products.dbdialog.json
│   │   │   └── DefaultRepo.Products.settings.json
│   │   └── client/        # Frontend Files
│   │       ├── a..lib/    # JavaScript libraries
│   │       ├── a..templates/  # Razor Templates
│   │       ├── a.Components/  # Generated Vue components
│   │       ├── a.SharedComponents/  # Shared components
│   │       ├── a.Layouts/  # Layouts
│   │       └── [AppName]/  # Created applications
│   └── appsettings.json   # Settings
└── README.md
```

### Important Files

#### Backend

- **workspace/server/DefaultRepo.Products.cs**:
  - C# code for methods related to Products

- **workspace/server/DefaultRepo.Products.dbdialog.json**:
  - Metadata for Products table
  - Columns, Relations, Queries, ClientUIs

- **workspace/server/DefaultRepo.Products.settings.json**:
  - Access and Cache settings for methods

#### Frontend

- **workspace/client/[AppName]/app.json**:
  - Application settings
  - Navigation
  - Themes

- **workspace/client/a.Components/Products_ReadList.vue**:
  - Generated Vue component for Products list

- **workspace/client/a..templates/ReadList.cshtml**:
  - Razor template for ReadList Component

### Reserved Folders

These folders are used by the Framework:
- `a..lib/`: JavaScript libraries
- `a..templates/`: Razor templates
- `a.Components/`: Generated components
- `a.SharedComponents/`: Shared components
- `a.Layouts/`: Layouts
- `appendstudio/`: Studio application

---

## Summary and Final Notes

### Important Notes for Developers

1. **Always call DynaCode.Refresh() after changing dynamic code**
2. **Use Cache for performance improvement**
3. **Check logs to identify issues**
4. **Use AccessRules for security**
5. **Customize UI with Templates**
6. **Use Relations to implement table relationships**

### Best Practices

1. **Place business logic code in Not Mapped methods**
2. **Use DbDialog for simple queries**
3. **Generate UI from Templates for maintainability**
4. **Use Namespace for code organization**
5. **Place settings in appsettings.json**

### Resources and References

- README.md: Initial guide
- Wiki: Extended documentation (https://github.com/mirshahreza/AppEnd/wiki)
- Existing code: Best resource for learning

---

## Complete Configuration Reference

### appsettings.json Settings

The `appsettings.json` file located in `AppEndHost` folder includes the following settings:

```json
{
  "AppEnd": {
    "TalkPoint": "talk-to-me",              // Endpoint for RPC
    "DefaultDbConfName": "DefaultRepo",      // Default database name
    "LogDbConfName": "DefaultRepo",          // Database for logs
    "LoginDbConfName": "DefaultRepo",        // Database for authentication
    "Secret": "YourSecretKey",               // Secret Key for Token
    "PublicKeyRole": "admin",                // Role with full access
    "PublicKeyUser": "admin",                // User with full access
    "IsDevelopment": false,                  // Development mode
    "EnableFileLogging": true,               // Enable File Logging
    "LogLevel": "Information",               // Log level (Debug, Information, Warning, Error)
    "LogsPath": "log",                       // Log files path
    "MaxLogFileSizeBytes": 2048,             // Max log file size (KB)
    "LogWriterQueueCap": 5,                  // Log writer queue capacity
    "PublicMethods": [                       // List of public methods
      "Zzz.AppEndProxy.PingMe",
      "Zzz.AppEndProxy.Login"
    ],
    "Serilog": {                             // Serilog settings
      "TableName": "BaseActivityLog",
      "Connection": "DefaultRepo",
      "BatchPostingLimit": 3,
      "BatchPeriodSeconds": 15
    },
    "DbServers": [                           // List of databases
      {
        "Name": "DefaultRepo",
        "ServerType": "MsSql",
        "ConnectionString": "YourConnectionString"
      }
    ]
  }
}
```

### Method Settings

Each method can have its own settings in the `.settings.json` file:

```json
{
  "Namespace.ClassName.MethodName": {
    "AccessRules": {
      "AllowedRoles": ["admin", "user"],
      "AllowedUsers": ["john"],
      "DeniedUsers": []
    },
    "CachePolicy": {
      "CacheLevel": "PerUser",
      "AbsoluteExpirationSeconds": 3600
    },
    "LogPolicy": "Full"
  }
}
```

**CacheLevel**:
- `None`: No caching
- `PerUser`: Per user caching
- `AllUsers`: Shared caching for all users

**LogPolicy**:
- `IgnoreLogging`: No logging
- `TrimInputs`: Log with trimmed inputs
- `Full`: Full logging with all inputs

---

## UI Widgets and Components

### Complete List of UI Widgets

AppEnd supports the following widgets:

#### Single Line Inputs
- **Textbox**: Single-line text input
- **DisabledTextbox**: Read-only text input
- **Sliderbox**: Numeric slider

#### Multi Line Inputs
- **MultilineTextbox**: Multi-line text input
- **Htmlbox**: HTML editor
- **CodeEditorbox**: Code editor

#### Select Inputs
- **Combo**: Dropdown list
- **Radio**: Radio buttons
- **ObjectPicker**: Select object from another table

#### Date & Time
- **DatePicker**: Date picker
- **DateTimePicker**: Date and time picker
- **TimePicker**: Time picker

#### Binary
- **ImageView**: Image upload and display
- **FileView**: File upload and display

#### Other
- **Checkbox**: Checkbox
- **ColorPicker**: Color picker
- **NoWidget**: No widget (hidden)

### Search Types

Search type can be set for each column:

- **None**: No search
- **Fast**: Fast search (for Combo and Radio)
- **Expandable**: Expandable search

### Auto Widget Selection

The framework automatically selects the appropriate widget based on column type:

- `INT IDENTITY` → `NoWidget`
- `BIT` → `Checkbox`
- `DATETIME` → `DateTimePicker`
- `DATE` → `DatePicker`
- `IMAGE` (Picture) → `ImageView`
- `IMAGE` (Other) → `FileView`
- `TEXT/NTEXT` → `MultilineTextbox`
- `NVARCHAR > 160` → `MultilineTextbox`
- `FK` → `Combo`
- Default → `Textbox`

---

## Compare Operators

For building WHERE clauses, the following operators are used:

- **Equal**: `=`
- **NotEqual**: `!=`
- **Contains**: `LIKE '%...%'`
- **StartsWith**: `LIKE '...%'`
- **EndsWith**: `LIKE '%...'`
- **MoreThan**: `>`
- **MoreThanOrEqual**: `>=`
- **LessThan**: `<`
- **LessThanOrEqual**: `<=`
- **In**: `IN (...)`
- **NotIn**: `NOT IN (...)`
- **IsNull**: `IS NULL`
- **IsNotNull**: `IS NOT NULL`

### Example of WHERE Usage

```json
{
  "Where": {
    "ConjunctiveOperator": "AND",
    "CompareClauses": [
      {
        "Name": "IsActive",
        "CompareOperator": "Equal",
        "Value": true
      },
      {
        "Name": "Name",
        "CompareOperator": "Contains",
        "Value": "test"
      }
    ]
  }
}
```

---

## Relations (Table Relationships)

### Types of Relations

#### 1. OneToMany
One-to-many relationship (Parent-Child)

```json
{
  "RelationName": "To_Orders_On_CustomerId",
  "RelationTable": "Orders",
  "RelationPkColumn": "Id",
  "RelationFkColumn": "CustomerId",
  "RelationType": "OneToMany",
  "CreateQuery": "Create",
  "ReadListQuery": "ReadList",
  "RelationUiWidget": "Grid"
}
```

#### 2. ManyToMany
Many-to-many relationship (with linking table)

```json
{
  "RelationName": "To_Tags_On_TagId",
  "RelationTable": "Tags",
  "RelationType": "ManyToMany",
  "LinkingTargetTable": "ProductTags",
  "LinkingColumnInManyToMany": "TagId",
  "RelationUiWidget": "CheckboxList"
}
```

### Relation UI Widgets

- **Grid**: Display in grid format
- **Cards**: Display in card format
- **CheckboxList**: Checkbox list (for ManyToMany)
- **AddableList**: Addable list

### File Centric Relations

For file-related relations (Image, File):
- `IsFileCentric: true`
- Use `Cards` widget for display

---

## ValueSharp System

ValueSharp is a system for determining default and processed values in Parameters.

### ValueSharp Expressions

#### 1. #Now
Current time:

```json
{
  "Name": "CreatedOn",
  "ValueSharp": "#Now"
}
```

#### 2. #Context:Key
Value from User Context:

```json
{
  "Name": "CreatedBy",
  "ValueSharp": "#Context:UserId"
}
```

#### 3. #Resize:ColumnName,Size
Image resizing:

```json
{
  "Name": "Picture_FileBody_xs",
  "ValueSharp": "#Resize:Picture_FileBody,75"
}
```

#### 4. #ToMD5:Value
Convert to MD5 Hash:

```json
{
  "Name": "Password",
  "ValueSharp": "#ToMD5:Password"
}
```

#### 5. #ToMD4:Value
Convert to MD4 Hash:

```json
{
  "Name": "Password",
  "ValueSharp": "#ToMD4:Password"
}
```

### Example in DbParam

```json
{
  "Params": [
    {
      "Name": "UpdatedBy",
      "DbType": "INT",
      "AllowNull": true,
      "ValueSharp": "#Context:UserId"
    },
    {
      "Name": "UpdatedOn",
      "DbType": "DATETIME",
      "AllowNull": true,
      "ValueSharp": "#Now"
    },
    {
      "Name": "Picture_FileBody_xs",
      "DbType": "IMAGE",
      "AllowNull": true,
      "ValueSharp": "#Resize:Picture_FileBody,75"
    }
  ]
}
```

---

## Package Manager

AppEnd includes a Package Manager system for packaging and sharing modules.

### Package Structure

A Package includes:
- `info.json`: Package information
- `install.sql`: Install script
- `uninstall.sql`: Uninstall script
- Other files (server objects, client components, ...)

### Package Fields

```json
{
  "Name": "MyPackage",
  "Title": "My Package Title",
  "Note": "Description",
  "Version": "1.0.0",
  "Url": "https://...",
  "CreatedBy": "admin",
  "CreatedOn": "2024-01-01",
  "UpdatedBy": "admin",
  "UpdatedOn": "2024-01-01",
  "InstallSql": "CREATE TABLE ...",
  "UnInstallSql": "DROP TABLE ...",
  "Installed": false,
  "MenuItems": []
}
```

### Working with Packages

1. **Create Package**:
   - In AppEndStudio, go to "Package Manager"
   - Click on "Create Package"
   - Enter Package information

2. **Export Package**:
   - Select Package
   - Export (to `.aepkg` file)

3. **Import Package**:
   - Click on "Upload Package"
   - Select `.aepkg` file

4. **Install Package**:
   - Select Package
   - Click on "Install"
   - SQL script will be executed

---

## Multiple Database Support

AppEnd supports multiple databases.

### Configuring Multiple Databases

```json
{
  "DbServers": [
    {
      "Name": "DefaultRepo",
      "ServerType": "MsSql",
      "ConnectionString": "Server=...;Database=MainDB;..."
    },
    {
      "Name": "ArchiveRepo",
      "ServerType": "MsSql",
      "ConnectionString": "Server=...;Database=ArchiveDB;..."
    }
  ]
}
```

### Using Different Databases

In the query name, use the following format:

```
{DbConfName}.{ObjectName}.{MethodName}
```

Example:
- `DefaultRepo.Products.ReadList`
- `ArchiveRepo.Orders.ReadList`

### Selecting Database in DbIO

```csharp
// Using default database
DbIO dbIO = DbIO.Instance();

// Using specific database
DbConf dbConf = DbConf.FromSettings("ArchiveRepo");
DbIO dbIO = DbIO.Instance(dbConf);
```

---

## RPC Client Library (append-client.js)

Client library for communication with Backend.

### Main Methods

#### rpc(options)
Send RPC request (Async):

```javascript
rpc({
  requests: [
    {
      Id: "req1",
      Method: "DefaultRepo.Products.ReadList",
      Inputs: {
        Pagination: { PageIndex: 0, PageSize: 10 }
      }
    }
  ],
  onDone: function(responses) {
    console.log(responses);
  },
  onFail: function(error) {
    console.error(error);
  },
  loadingModel: "..."  // Show Loading
});
```

#### rpcSync(options)
Send RPC request (Sync):

```javascript
let responses = rpcSync({
  requests: [...]
});
```

#### rpcAEP(method, inputs, onDone, onFail)
Simple method call:

```javascript
rpcAEP("DefaultRepo.Products.ReadList", 
  { Pagination: { PageIndex: 0, PageSize: 10 } },
  function(result) {
    console.log(result);
  }
);
```

### Cache in Client

The client automatically caches responses:
- If a similar request is sent
- It uses the cache
- No need to send the request

---

## Frontend Application Structure

### Structure of app.json

```json
{
  "title": "Application Title",
  "sub-title": "Subtitle",
  "dir": "ltr",                    // ltr or rtl
  "lang": "en",                    // Default language
  "calendar": "Gregorian",         // Calendar
  "defaultComponent": "components/Home",
  "navigation": [
    {
      "title": "Section Title",
      "icon": "fa-solid fa-fw fa-icon",
      "items": [
        {
          "title": "Item Title",
          "icon": "fa-solid fa-fw fa-icon",
          "component": "components/MyComponent"
        }
      ]
    }
  ],
  "themes": {
    "default": "theme-name"
  }
}
```

### Layouts

Main layouts available:

1. **BO.vue**: Back Office Layout
   - Sidebar Menu
   - Header
   - Content Area

2. **FO.vue**: Front Office Layout
   - No Sidebar
   - Simple Header
   - Content Area

### Shared Components

Shared components available in `a.SharedComponents/`:

- **BaseComponentLoader**: Component loader
- **BaseContent**: Main content display
- **BaseFileEditor**: File editor
- **BaseJsonView**: JSON viewer
- **BaseConfirm**: Confirm dialog
- **BasePrompt**: Prompt dialog
- **Login**: Login page
- **MyProfile**: User profile
- **SideMenu**: Simple side menu
- **SideMenu2Level**: Two-level side menu
- and ...

### Application Structure

#### app.json

Settings file for each application:

```json
{
  "title": "Application Title",
  "sub-title": "Subtitle",
  "dir": "ltr",                    // ltr or rtl
  "lang": "en",                    // Default language
  "calendar": "Gregorian",         // Calendar (Gregorian, Jalali)
  "defaultComponent": "components/Home",
  "translation": {                 // Translations
    "Home": "خانه",
    "Products": "محصولات"
  },
  "navigation": [
    {
      "title": "Section",
      "icon": "fa-icon",
      "items": [
        {
          "title": "Item",
          "icon": "fa-icon",
          "component": "components/MyComponent",
          "params": "&param=value"
        }
      ]
    }
  ]
}
```

**Important Fields**:
- `title`: Application title
- `sub-title`: Subtitle
- `dir`: Text direction (ltr/rtl)
- `lang`: Language
- `calendar`: Calendar type
- `defaultComponent`: Default component
- `translation`: Translation dictionary
- `navigation`: Navigation structure

### Component Structure

#### Structure of a Vue Component

```vue
<template>
    <!-- HTML Template -->
    <div>
        <h1>{{ title }}</h1>
    </div>
</template>

<script>
let _this = {
    cid: "",
    c: null,
    inputs: {},
    local: {
        // Local Data
    }
};

export default {
    props: {
        cid: String,
        // Other Props
    },
    setup(props) {
        _this.cid = props.cid;
        _this.inputs = shared["params_" + _this.cid] || {};
    },
    data() {
        return _this;
    },
    created() {
        _this.c = this;
    },
    mounted() {
        // Initialization
        initVueComponent(_this);
    },
    methods: {
        // Methods
    }
}
</script>

<style scoped>
/* Styles */
</style>
```

#### Receiving Parameters

Parameters are received from Query String or when opening the Component:

```javascript
// From Query String
let param = shared.getQueryString('paramName');

// From inputs (when Component is opened)
_this.inputs = shared["params_" + _this.cid];
```


### RPC Client Library (جزئیات)

#### ساختار RPC Request

```javascript
{
    Id: "unique-id",
    Method: "DefaultRepo.Products.ReadList",
    Inputs: {
        ClientQueryJE: {
            QueryFullName: "DefaultRepo.Products.ReadList",
            Pagination: {
                PageNumber: 1,
                PageSize: 10
            },
            Where: {
                CompareClauses: [...]
            }
        }
    }
}
```

#### ساختار RPC Response

```javascript
{
    Id: "unique-id",
    Duration: 125,
    IsSucceeded: true,
    FromCache: false,
    Result: {
        Master: [
            { Id: 1, Name: "Product 1" },
            { Id: 2, Name: "Product 2" }
        ],
        Aggregations: {...}
    }
}
```

#### Cache in Client

The client automatically caches responses:

```javascript
function analyzeRequests(requests) {
    let cached = [];
    let todo = [];
    
    requests.forEach(req => {
        let cacheKey = generateCacheKey(req);
        if (cache.has(cacheKey)) {
            cached.push(cache.get(cacheKey));
        } else {
            todo.push(req);
        }
    });
    
    return {
        cachedResponses: cached,
        todoRequests: todo
    };
}
```

### Templates System

#### ساختار Template

Template‌ها فایل‌های Razor (.cshtml) هستند که Vue Component تولید می‌کنند.

**مثال Template**:
```razor
@{
    string objectName = Model.DbDialog.ObjectName;
    string loadAPI = Model.ClientUI.LoadAPI;
}
<template>
    <div class="card">
        <div class="card-header">
            <h1>@objectName</h1>
        </div>
        <div class="card-body">
            <!-- Vue Template Code -->
            <div v-for="row in data">
                {{ row.Name }}
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            data: []
        }
    },
    mounted() {
        // Load data
    }
}
</script>
```

#### Template Variables

در Template می‌توان از `Model` (BuildInfo) استفاده کرد:

- `Model.DbDialog`: DbDialog Object
- `Model.ClientUI`: ClientUI Object
- `Model.Parameters`: Dictionary اضافی

#### Template Helpers

Helper Methods موجود در Template:

- `Model.GetDisplayColumns(column)`: ستون‌های نمایشی
- `Model.GetTargetPkColumn(column)`: PK Column
- `Model.GetFirstFileFieldName()`: نام فیلد فایل

### Translation System

#### نحوه کار

ترجمه‌ها در فایل `app.json` در بخش `translation` ذخیره می‌شوند:

```json
{
  "translation": {
    "Home": "خانه",
    "Products": "محصولات",
    "Create": "ایجاد"
  }
}
```

#### استفاده در Components

```vue
<template>
    <div>
        <h1>{{ shared.translate("Home") }}</h1>
    </div>
</template>
```

```javascript
shared.translate("Products"); // "محصولات"
```

#### Extract Translation Keys

می‌توانید به صورت خودکار کلیدهای ترجمه را از Components استخراج کنید:
- در AppEndStudio به "Themes" → "Translation Management" بروید
- روی "Extract Keys" کلیک کنید
- کلیدهای موجود در Components استخراج می‌شوند

### PWA Support

#### Service Worker

AppEnd از Service Worker برای PWA پشتیبانی می‌کند:

```javascript
// serviceWorker.js
self.addEventListener('install', evt => {
    // Cache assets
});

self.addEventListener('fetch', evt => {
    // Serve from cache
});
```

#### Manifest

فایل `manifest.json`:

```json
{
  "short_name": "AppEnd",
  "name": "AppEnd",
  "icons": [...],
  "start_url": ".",
  "display": "standalone",
  "theme_color": "#000000",
  "background_color": "#ffffff"
}
```

### Styling System

#### CSS Files

1. **append-all-ltr.css**: Bundle CSS برای LTR
2. **append-all-rtl.css**: Bundle CSS برای RTL
3. **append-client.css**: استایل‌های مخصوص کلاینت
4. **custom.css**: استایل‌های سفارشی هر اپلیکیشن

#### Bootstrap Integration

AppEnd از Bootstrap استفاده می‌کند:
- Classes: `btn`, `card`, `form-control`, ...
- Utilities: `text-center`, `mb-3`, `d-flex`, ...
- Components: Modal, Toast, Dropdown, ...

#### Custom CSS Variables

می‌توان از CSS Variables استفاده کرد:

```css
:root {
    --bs-primary: #0d6efd;
    --bs-primary-rgb: 13, 110, 253;
}
```

### Widget System

#### Data Attributes

می‌توان از Data Attributes برای فعال‌سازی Widget‌ها استفاده کرد:

```html
<div data-ae-widget="datepicker" 
     data-ae-widget-options='{"format":"YYYY-MM-DD"}'>
</div>
```

**Widgets موجود**:
- `datepicker`: Date Picker
- `datetimepicker`: DateTime Picker
- `select2`: Enhanced Select
- `fileupload`: File Upload
- و ...

### Modal System

#### باز کردن Modal

```javascript
shared.openComponent("/a.Components/MyComponent", {
    title: "Modal Title",
    modalSize: "modal-lg",
    showHeader: true,
    showFooter: false,
    resizable: true,
    draggable: true,
    closeByOverlay: true,
    params: {
        param1: "value1"
    }
});
```

**Options**:
- `title`: عنوان Modal
- `modalSize`: اندازه (`modal-sm`, `modal-lg`, `modal-xl`, `modal-fullscreen`)
- `showHeader`: نمایش Header
- `showFooter`: نمایش Footer
- `resizable`: قابل تغییر اندازه
- `draggable`: قابل Drag
- `closeByOverlay`: بستن با کلیک روی Overlay
- `params`: پارامترها برای کامپوننت

### File Structure Details

#### a..lib/ (کتابخانه‌ها)

این پوشه شامل تمام کتابخانه‌های JavaScript و CSS است.

**کتابخانه‌های اصلی**:
- **jQuery 3.7.1**: کتابخانه اصلی DOM Manipulation
- **Bootstrap 5**: Framework CSS
- **Vue.js 3**: Framework JavaScript
- **Lodash**: Utility Functions
- **Ace Editor**: Code Editor
- **Font Awesome**: Icons
- **Moment.js**: Date/Time Library
- **Jalaali.js**: Jalali Calendar
- **Trumbowyg**: WYSIWYG Editor
- **Croppie**: Image Cropper
- **jstree**: Tree View

#### a..templates/ (Template‌ها)

Template‌های Razor که Vue Components تولید می‌کنند:

1. **ReadList.cshtml**: لیست داده‌ها
2. **Create.cshtml**: فرم ایجاد
3. **UpdateByKey.cshtml**: فرم ویرایش
4. **ReadByKey.cshtml**: نمایش یک رکورد
5. **ReadTreeList.cshtml**: لیست درختی
6. **BaseEmptyComponent.cshtml**: کامپوننت خالی
7. **FormColumnContent.cshtml**: محتوای ستون فرم
8. **FormRelation*.cshtml**: Relation Forms

#### a.Components/ (کامپوننت‌های تولید شده)

کامپوننت‌های Vue که از Template‌ها تولید شده‌اند:

**Naming Convention**:
- `{ObjectName}_{QueryType}.vue`
- مثال: `Products_ReadList.vue`, `Orders_Create.vue`

#### a.SharedComponents/ (کامپوننت‌های مشترک)

کامپوننت‌هایی که در همه جا قابل استفاده هستند:

1. **BaseComponentLoader.vue**: لودر کامپوننت
2. **BaseContent.vue**: نمایش محتوا
3. **BaseFileEditor.vue**: ویرایشگر فایل
4. **BaseJsonView.vue**: نمایش JSON
5. **BaseConfirm.vue**: Confirm Dialog
6. **BasePrompt.vue**: Prompt Dialog
7. **Login.vue**: صفحه لاگین
8. **SideMenu.vue**: منوی کناری
9. **SideMenu2Level.vue**: منوی دو سطحه
10. **MyProfile.vue**: پروفایل کاربر
11. **ImageEditor.vue**: ویرایشگر تصویر
12. **DbObjectPicker.vue**: انتخاب DbObject
13. و ...

#### a.Layouts/ (Layout‌ها)

Layout‌های اصلی:

1. **BO.vue**: Back Office Layout (با Sidebar)
2. **FO.vue**: Front Office Layout (ساده)

### Component Communication

#### Parent-Child Communication

From Props and Events:

```vue
<!-- Parent -->
<component-loader 
    src="components/Child" 
    cid="child"
    :someProp="value" />

<!-- Child -->
<script>
export default {
    props: {
        someProp: String
    },
    methods: {
        notifyParent() {
            this.$emit('event-name', data);
        }
    }
}
</script>
```

#### Global Communication

Using `shared` object:

```javascript
// Set global data
shared.globalData = { ... };

// Get global data
let data = shared.globalData;
```

#### Session Storage

For storing data in Session:

```javascript
sessionStorage.setItem('key', JSON.stringify(data));
let data = JSON.parse(sessionStorage.getItem('key'));
```

### Form Validation

#### Validation System

Using Data Attributes for Validation:

```html
<input 
    type="text" 
    data-ae-validation-required="true"
    data-ae-validation-rule=":=s(0,100)" />
```

#### Inputs Regulator

```javascript
this.regulator = $(`#${this.cid}`).inputsRegulator();

// Validate
if (this.regulator.isValid()) {
    // Submit
}
```

### Image Handling

#### Displaying Images

```javascript
// From Bytes to URI
let imageUri = shared.getImageURI(imageBytes);

// In Template
<img :src="shared.getImageURI(row.Picture)" />
```

#### Image Editor

Use `ImageEditor.vue`:

```javascript
shared.openComponent("/a.SharedComponents/ImageEditor", {
    params: {
        imageBytes: imageData,
        callback: function(editedImage) {
            // Handle edited image
        }
    }
});
```

### Calendar System

#### Supporting Different Calendars

- **Gregorian**: میلادی
- **Jalali**: شمسی

**استفاده**:
```javascript
// با توجه به تنظیمات اپلیکیشن
shared.formatDateL(date, getAppConfig()["calendar"]);
```

### Keyboard Shortcuts

#### Managing Shortcuts

```javascript
// Receive Shortcuts
let shortcuts = shared.getUserShortcuts();

// Set Shortcut
shared.setUserShortcuts([
    { key: "Ctrl+S", action: "save" }
]);
```

### Working Indicators

#### Loading States

```javascript
// Show Loading
let workingId = shared.showWorking(shared.heavyWorkingCover);

// Hide Loading
shared.hideWorking(workingId);
```

**انواع Loading Cover**:
- `shared.heavyWorkingCover`: Loading کامل صفحه
- `shared.notHeavyWorkingCover`: Loading شفاف
- `shared.miniHeavyWorkingCover`: Loading کوچک

### Toast Notifications

#### Displaying Messages

```javascript
// Success
shared.showSuccess("Operation completed!");

// Error
shared.showError("Something went wrong!");

// Warning
shared.showWarning("Please check!");

// Info
shared.showInfo("Information message");
```

### Component Lifecycle

#### Lifecycle Hooks

```vue
<script>
export default {
    setup(props) {
        // Setup Phase
    },
    created() {
        // Component Created
    },
    mounted() {
        // Component Mounted
        initVueComponent(this);
    },
    beforeUnmount() {
        // Cleanup
    }
}
</script>
```


