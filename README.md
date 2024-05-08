# Connecting to SQL Server from AutoCAD: A Step-by-Step Guide

AutoCAD stands out as a powerhouse tool used by engineers, architects, and designers worldwide. Its extensibility through plugins opens up a world of possibilities for customization and integration with other software systems. One common integration scenario is connecting AutoCAD to a SQL Server database to leverage data in drawings or perform database operations directly from within AutoCAD.

With advent of Modern .NET platform, we have recieved some querys from customers about loading the SQL data client libaries on to AutoCAD runtime.

This post aims to address the query and at same time guides on connect to SQL Server.



## Connecting to SQL Server from AutoCAD:

AutoCAD plugins are typically developed using the .NET framework, which offers robust support for database connectivity through ADO.NET. 

However with AutoCAD 2025 onwards which is built on .NET 8.0, for the modern platform, we'll utilize the `System.Data.SqlClient` namespace, which provides classes for interacting with SQL Server databases.



Before diving into the code, it's crucial to ensure that the plugin targets the correct platform, especially when dealing with external dependencies like `System.Data.SqlClient`. AutoCAD plugins should target the Windows x64 platform to align with AutoCAD's runtime environment. To achieve this, we specify `<RuntimeIdentifier>win-x64</RuntimeIdentifier>` in the project file (csproj) to instruct the build system to generate a DLL compatible with the Windows x64 platform.



```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  </PropertyGroup>

  <!-- Other project settings and dependencies -->

</Project>

```

With the project configured to target Windows x64, we can proceed to write code to connect to SQL Server and execute queries from within AutoCAD.

Connecting to SQL Server and Running Queries:
We'll split the code into two segments for clarity: one for testing the SQL connection and another for executing queries and interacting with AutoCAD's editor.

- Test Server Connection

```csharp
public bool TestSqlServerConnection()
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            return true;
        }
    }
    catch (System.Exception ex)
    {
        Console.WriteLine($"Cannot connect to Database server: {ex.Message}");
        return false;
    }
}
```

- Run Query Strings

```csharp
using (var cmd = new SqlCommand(queryString, connection))
{
    using (var reader = cmd.ExecuteReader())
    {
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                string city = reader.GetString(0); // assuming city is the first column (index 0)
                int customerCount = reader.GetInt32(1); // assuming number of customers is the second column (index 1)
                ed.WriteMessage($"\nCity: {city}, Customer Count: {customerCount}");
            }
        }
        else
        {
            ed.WriteMessage("\nNo results found.");
        }
    }
}
```

- AutoCAD Client code

```csharp
 var doc = Application.DocumentManager.MdiActiveDocument;
 if(doc is null)
 {
     return;
 }   
 var ed = doc.Editor;
 // Consider storing connection string in a configuration file for security
 string connectionString = "your connection string";
 var data = new DataAccessor(connectionString);
 try
 {
     data.TestSqlServerConnection();
     ed.WriteMessage("\nConnected to SQL Server database successfully!");
     data.RunQueryAndWriteToEditor(ed);
 }
 catch (System.Exception ex)
 {
     ed.WriteMessage($"\nConnecting to SQL Server database failed!\n{ex.Message}");
 }
```

### To Build

Through CLI

```bash
git clone https://github.com/MadhukarMoogala/ACAD-SQLPlugin.git
cd ACAD-SQLPlugin
set ArxSdk=<Your SDK>
set AcadDir= <Your AutoCAD Location>
dotnet build ACAD-SQLPlugin.csproj -a x64 -c Debug
```

Through UI

```bash
git clone https://github.com/MadhukarMoogala/ACAD-SQLPlugin.git
cd ACAD-SQLPlugin
devenv ACAD-SQLPlugin.csproj
```

Edit `ACAD-SQLPlugin.csproj` file to fix <AcadDir> and <ArxSdk>

## 

## DEMO

- After netload the `bin\x64\Debug\net8.0-windows\win-x64\ACAD-SQLPlugin.dll`

- Run `ConnectDB` command

![image](https://github.com/MadhukarMoogala/ACAD-SQLPlugin/assets/6602398/0dbbe6ec-274e-44db-8800-d831db0e8214)


## LICENSE

[MIT](https://github.com/MadhukarMoogala/ACAD-SQLPlugin/blob/main/LICENSE.txt)

#### Written By

Madhukar Moogala, APS
