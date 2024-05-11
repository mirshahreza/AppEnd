<p align="center" width="100%">
    <img src="https://github.com/mirshahreza/AppEnd/blob/master/AppEndHost/workspace/client/a..lib/images/AppEnd-Logo-Full.png?raw=true" />
</p>
  
**What is AppEnd?**
> AppEnd is a **Low Code** and Rapid Application Development (**RAD**) Environment.  

**Why AppEnd?**
>As you know there are several RAD tools, so why should you use the AppEnd?  
- It is open source and freeware  
- Easy to use with a low learning curve  
- Really clean, simple and modular architecture  
- It can host on Linux & Windows  
- It is not just for Database IO, It is a platform to develop anything and a fullstack application host  
- The framework structure is developer friendly and is based on general development standards  
- You can easily inject your custom code in client and server components  
- User Interfaces and backends are fully customizable  
- User Interfaces are based on Bootstrap & VueJs (They are easy to learn and use)  
- User Interfaces are based on translation files so you can have applications in multiple languages  
- Easily inspect database structure and create applications based on it  
- You can manage and use APIs directly in other applications  
- Initial full stack CRUD scenarios can generate by some simple clicks :)  
- AppEnd can be a platform to develop back office and front office parts  
- BuiltIn module to manage deployment tasks  
- Can deploy single or multi node  
- It is under development and is a live project  

**Technology**  
Host: Linux Or Windows  
Application Server: .Net Core / C#  
Database: MsSql  
Client: SPA based on Bootstrap & VueJs 3  

**Roadmap**  
Database centric applications must to have at least below sections considering users access levels  
1- Application builder & CRUD functionalities: In progress  
    - Make Tables and Lists responsive  
    - New UI widgets : To make Create-Update forms better  
    - More advanced searchbars for generated Lists  
    - UI designer  
    - Docker image : To easy installation  
    - Package Manager : To create/import/export packages as portable plugins  
    - Git : To manage your production  
    - OpenId (SSO)  
    - Task Scheduler  
2- Workflow Engine: Planning  
3- Repotting and Visualization system: Planning  

So we will dive into phase 2 and 3 after phase 1 becomes stable enough.

**Getting Started Guide**  
To run the project  
1- Clone the repository  
2- Open it by Visual Studio 2022  

3- Setup MSSQL Server database, to setup database  
     3-1 Create an empty database in your sql server instance  
     3-2 Get the [Zzz_Deploy](https://github.com/mirshahreza/RDBMS-PackageManager/blob/master/MsSql/Zzz_Deploy.sql) producer  
     3-3 Add the above producer to the database  
     3-4 Running the following commands in SQL Server Management Studio to Initiate your database:

     EXEC Zzz_Deploy  
     EXEC Zzz_Deploy 'AppEnd'  

4- Change the database connection string at the 'AppEnd\AppEndHost\appsettings.json'

5- Run the 'AppEndHost' project  
    (Default Username is Admin and Password is P#ssw0rd)

**Documentation**  
For more information about AppEnd go to documentation [wiki](https://github.com/mirshahreza/AppEnd/wiki)  

**Support**  
To support me you can  
1- Participate in development  
2- Donate  

