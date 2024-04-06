<div style="text-align:center">
    <img src="https://github.com/mirshahreza/AppEnd/blob/master/AppEndHost/workspace/client/..lib/images/AppEnd-Logo-Full.png?raw=true" />
</div>
  
**What is AppEnd?**
> AppEnd is a **Low Code** and Rapid Application Development (**RAD**) Environment.  

**Why AppEnd?**
>As you know there are several RAD tools, so why should you use the AppEnd?  
Because:
- It is open source and freeware
- Easy to use with a low learning curve
- Really clean, simple and modular architecture
- It can host on Linux & Windows
- Target Database can be MsSql, Oracle, Postgress, MySql (Currently implemented for MsSql)
- The framework structure is developer friendly and is based on general development knowledge
- You can easily inject your custom code in client and server components
- User Interfaces and backends are fully customizable
- User Interfaces are based on Bootstrap & VueJs (They are easy to learn and use)  
- User Interfaces are based on translation files so you can have applications in multiple languages  
- Easily inspect database structure and create applications based on it  
- You can manage and use APIs directly in other applications  
- Initial full stack CRUD scenarios can generate by some simple clicks :)  
- By using built-in libraries in client and server you can make anything  
- AppEnd can be a platform to develop back office and front office parts    
- It is under development and is a live project  

**Technology**  
Host: Linux Or Windows  
Application Server: .Net Core / C#  
Database: MsSql  
Client: SPA based on Bootstrap & VueJs 3  

**Roadmap**  
Database centric applications must to have at least below sections considering users access levels  
1- CRUD functionalities: In progress  
2- Workflow Engine: Planning  
3- Repotting and Visualization system: Planning  

So we will dive into it after phase 1 become stable enough.

**Getting Started Guide**  
To run the project  
1- Clone the repository  
2- Open it by Visual Studio 2022  
3- Setup database, to setup database  
&nbsp;&nbsp;- Create an empty database in your sql server instance  
&nbsp;&nbsp;- Get the producer at https://github.com/mirshahreza/RDBMS-PackageManager/blob/master/MsSql/Zzz_Deploy.sql  
&nbsp;&nbsp;- Add the above producer to the database  
&nbsp;&nbsp;- Initiate your database by running the following commands in SQL Server Management Studio  
&nbsp;&nbsp;&nbsp;&nbsp;> EXEC Zzz_Deploy  
&nbsp;&nbsp;&nbsp;&nbsp;> EXEC Zzz_Deploy 'AppEnd'  
4- Change the database connection string at the appsettings.json  
5- Run the project  

**Documentation**  
For more information about AppEnd go to documentation wiki https://github.com/mirshahreza/AppEnd/wiki.

**Donation**  
By your donations we can develop the AppEnd continues.  
If you want to donate to support the development of this software, please contact me.  
