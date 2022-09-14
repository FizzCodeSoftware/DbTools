# Fizzcode DbTools
###### Fizzcode DbTools open source project

The two main usage of DbTools is
- Describe a database schema and properties in C# code, with the ability to generate the database on multiple database engines
- Provide a command line tool, DbTools.Console to execute DbTools functions (examples: generating database documentation or comparison to an Excel file, generate database definition as .cs files)

# DatabaseDeclaration
A very simple database definition looks like this (from
[TestDatabaseSimpleTyped.cs](https://github.com/FizzcodeSoftware/DbTools/blob/master/Tests/DbTools.DataDefinition.Tests/DatabaseDeclaration/Typed/TestDatabaseSimpleTyped.cs)):

```cs
namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseSimpleTyped : TestDatabaseDeclaration
    {
        public Company Company { get; } = new Company();
    }

    public class Company : SqlTable
    {
        public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();

        public SqlColumn Name { get; } = Generic1.AddNVarChar(100);
    }
}
```

# DbTools.Console usage
Build, and run FizzCode.DbTools.Console.exe
(DbTools\DbTools.Console\bin\Release\net6.0\FizzCode.DbTools.Console.exe)

Option examples:

> ```FizzCode.DbTools.Console.exe document --connectionString "<connection string>" --sqlType MsSql2016 --patternFileName "" --flags None```

> ```FizzCode.DbTools.Console.exe changedocument --connectionStringOriginal "<connection string>" --connectionStringNew "<connection string>" --sqlTypeOriginal MsSql2016 --sqlTypeNew MsSql2016 --patternFileName "" --patternFileNameOriginal "" --patternFileNameNew "" --flags None```

# License

See LICENSE.

# Contributing

Regarding pull requests or any contribution
- we need a signed CLA (Contributor License Agreement)
- only code which comply with .editorconfig is accepted

# Projects
## DbTools.Console
Console project which accepts commands to execute DbTools features.
## DbTools.DataDefinition
Contains classes for database definitions.
## DbTools.DataDefinitionDocumenter
Generates database documentation from DatabaseDefinition.
## DbTools.DataDefinitionExecuter
Executes the database creation sql commands from the DataDefinitionGenerator, to create the database.
## DbTools.DataDefinitionGenerator
Generates executable sql to create database from DatabaseDefinition.
## DbTools.DataDefinitionReader
Builds DatabaseDefinition from an existing database.
## DbTools.DataGenerator
Generates data, typically for testing purposes, based on DatabaseDefinition.
## DbTools.TestBase
Contains helper classes for unit testing DbTools.

# Development setup
## Config files
Configuration values are stored in config.json files.
Configuration values for tests are stored in testconfig.json files.

You can use local config values to override content of these files, which are local (developer specific), or not desired to be included in source control (for example, a connection string with username and password to an internal environment).
Use naming as config-local.json and testconfig-local.json.

## Running Unit tests
To run unit tests in parallel between assemblies, you have to enable it by selecting Test Explorer / Configure Run Settings / Select solution wide runsettings file, and then select the file \Tests\test.runsettings

See https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?branch=release-16.4&view=vs-2022