Employee / Personal Tracking System
C# WPF, Microsoft SQL server, EF-Core, LINQ, Visual Studio 2022

It is an Employee/Personal management system with CRUD operations.
-User/Admin has to login with UserNo and Password.
-Admin can add, delete, and update users, tasks for users, salaries of users, departments, positions, and permissions in a system.
-User can view its tasks and deliver the task.

WPF - windows presentation foundation
-used to create desktop applications.
-we have created windows, pages, and user controls with the GRID system.
-user controls used to switch layouts/windows in the main window.

Entity Framework
-We have used an entity framework with a database-first approach.
-We created the database with all tables and defined the relation between tables(foreign keys), then scaffolded EF code in Visual Studio.
-Run command in package manager console "Scaffold-DBContext "server=; database=; trusted_connection=True;" Microsoft.EntityFrameworkCore.SqlServer -outputDir DB"
-It will create all tables in a model form in your code and also create a context class that can be used for all database operations.
-Create an object of context class and perform operations on its public members which are tables. (Add, Update, Remove)

LINQ language integrated query
-C# feature that can be used to query objects
-we are using LINQ to perform join operations on tables and selecting/extracting related data from tables.

Improvments
-Refactor code for reusability and expansion.
-UI improvements.
-handling errors and exceptions.
