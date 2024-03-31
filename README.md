**What is AppEnd?**
> AppEnd is a **Low Code** Rapid Application Development (**RAD**) tools.  

**Why AppEnd?**
>As you know there are several RAD tools, so why should you use the AppEnd?  
Because:
- It is open source and freeware
- It is under development and is a live project  
- It can host on Linux & Windows
- The framework structure is developer friendly and is based on normal development knowledge
- User Interface is based on Bootstrap & VueJs
- User Interface is fully customizable
- User Interface is based on translation files so you can have applications in multiple languages
- You can easily inject client and server code
- Easily inspect database structure and create applications based on it
- Target Database can be MsSql, Oracle, Postgress, MySql (Currently implemented for MsSql)
- You can manage and use APIs directly in other applications
- Useful libraries in client and server to make anything

**Technology**  
Host: Linux Or Windows  
Application Server: .Net Core / C#  
Database: MsSql  
Client: SPA based on Bootstrap & VueJs 3  

**Roadmap**  
Database centric applications must to have at least below sections considering users access levels  
1- CRUD: Currently we are in this phase  
2- Workflow Engine: Planning  
3- Repotting and Visualization system: Planning  

So we will dive into it after phase 1 become stable enough.

**Getting Started Guide**  
To run the project  
1- Clone the repository  
2- Open it by Visual Studio 2022  
3- Setup database, to setup database  
	- Create an empty database in your sql server instance  
	- Get the producer at https://github.com/mirshahreza/RDBMS-PackageManager/blob/master/MsSql/Zzz_Deploy.sql  
	- Add the above producer to the database  
	- Initiate your database by running the following commands in SqlServer Management Studio  
		- EXEC Zzz_Deploy  
		- EXEC Zzz_Deploy 'AppEnd'  
4- Change the database connection string at the appsettings.json  
5- Run the project  

**Documentation**  
For more information about AppEnd go to documentation (Under Construction).

**Donation**  
By your donations we can develop the AppEnd continues.  
If you want to donate to the development of this software, please contact me.  



