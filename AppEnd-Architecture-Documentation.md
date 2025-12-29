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

---

## Introduction

AppEnd is a Low-Code and Rapid Application Development (RAD) framework that helps you quickly build complete Full-Stack applications. This framework is designed with a simple and modular architecture with high development and customization capabilities.

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

## Overall System Architecture

### General Architecture

AppEnd uses a three-layer architecture:

```
???????????????????????????????????????????
?         Frontend (Vue.js + Bootstrap)   ?
?  - SPA Application                      ?
?  - Components & Templates               ?
???????????????????????????????????????????
                  ? RPC (JSON over HTTP)
???????????????????????????????????????????
?         AppEndHost (ASP.NET Core)       ?
?  - HTTP Server                          ?
?  - RPC Endpoint Handler                 ?
?  - Static File Server                   ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?      AppEndServer (Business Logic)      ?
?  - RpcNet (RPC Handler)                 ?
?  - Dynamic Code Execution               ?
?  - Service Layer                        ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?     AppEndDbIO (Data Access Layer)      ?
?  - Database Connection                  ?
?  - Query Builder                        ?
?  - CRUD Operations                      ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?         SQL Server Database             ?
???????????????????????????????????????????
```

### Communication Between Layers

#### Frontend to Backend Communication

Frontend communicates with Backend via **RPC (Remote Procedure Call)**:

- **Endpoint**: `/talk-to-me` (configurable in appsettings.json)
- **Method**: POST
- **Format**: JSON
- **Authentication**: Token-based (in Header)

#### Request Processing

1. Frontend sends RPC request
2. `RpcNet` receives the request
3. `DynaCode` finds and executes the corresponding method
4. Method can use `DbIO` to access database
5. Result is returned to Frontend

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

### 2. AppEndDbIO

**Responsibility**: Database access and query building

**Main Classes**:
- `DbIO.cs`: Base class for database communication
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

### 3. AppEndDynaCode

**Responsibility**: Dynamic C# code compilation and execution

**Features**:
- Compile `.cs` files in `workspace/server`
- Create dynamic assembly
- Dynamically invoke methods
- Access control management
- Result caching
- Automatic logging

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

### 5. AppEndHost

**Responsibility**: Application hosting and HTTP Server

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
- `a..templates/`: Razor templates for UI generation
- `a.Components/`: Generated Vue components
- `a.SharedComponents/`: Shared Vue components
- `a.Layouts/`: Vue layouts
- `AppEndStudio/`: Studio application for management

## Workflow

### Flow of a Request from Frontend to Database

1. User performs an Action in Frontend
2. Frontend creates an RPC Request
3. Request is sent to /talk-to-me
4. RpcNet receives the request
5. Actor (user) is extracted from Token
6. DynaCode finds the method and checks access
7. Method is executed (may use Cache)
8. Method may create and execute ClientQuery
9. DbIO executes SQL query
10. Result is returned from Database
11. Result is processed and converted to JSON
12. Response is returned to Frontend
13. Frontend updates UI

## Key Features

### 1. Authentication & Authorization

**Authentication**:
- Token-based Authentication
- Token is sent in Header with name `token`
- Token contains user information (Id, UserName, Roles)

**Authorization**:
- Method level: Each method can have Access Rules
- Role level: Access based on Role
- User level: Access based on UserName
- Public Methods: Public methods that don't require authentication

### 2. Dynamic Code Compilation

- C# code in `workspace/server` is dynamically compiled
- Uses Roslyn for compilation
- Can Refresh without restarting application

### 3. Query Builder

- Build SQL query from JSON
- Supports WHERE clauses, ORDER BY, Pagination, Aggregations, Relations

### 4. Template Engine

- Uses RazorEngine for UI generation
- Automatic Vue Components generation

### 5. Caching

- Memory Cache for method results
- Configurable Cache Policy
- Configurable TTL

### 6. Logging

- Automatic logging for all methods
- Save to Database (BaseActivityLog)

### 7. File Management

- Static file management
- Upload/Download support

## How to Create an Application

### Step 1: Initial Setup

1. **Database Setup**:
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
       "Secret": "YourSecretKey"
     }
   }
   ```

3. **Run Application** and login with Username: `Admin` and Password: `P#ssw0rd`

### Step 2: Create a New Table

1. Create table in database
2. In AppEndStudio, select the table
3. Click "Create Server Objects"

### Step 3: Create Queries

Create queries like ReadList, Create, UpdateByKey, DeleteByKey

### Step 4: Generate UI

1. Create ClientUI for each query
2. Click "Build UI"

### Step 5: Create Frontend Application

1. Create folder in `workspace/client/`
2. Create `app.json` with navigation
3. Access your application

## Development Guide

### Creating a Custom Method

```csharp
namespace MyNamespace
{
    public static class MyClass
    {
        public static object? MyMethod(AppEndUser? Actor, string Parameter)
        {
            // Your code
            return new { Result = "Success" };
        }
    }
}
```

### Working with Database

```csharp
public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
{
    return ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
}
```

### Creating Vue Components

```vue
<template>
    <div>
        <h1>My Component</h1>
    </div>
</template>

<script>
export default {
    data() {
        return { }
    }
}
</script>
```

---

**Version**: 2.1  
**Date**: 2024
