# Fizzcode DbTools
###### Fizzcode DbTools open source project

# License

See LICENSE.

# Contributing

Regarding pull requests or any contribution
- we need a signed CLA (Contributor License Agreement)
- only code which comply with .editorconfig is accepted

## Projects
### DbTools.Console
Console project which accepts commands to execute DbTools features.
### DbTools.DataDefinition
Contains classes for database definitions.
### DbTools.DataDefinitionDocumenter
Generates database documentation from DatabaseDefinition.
### DbTools.DataDefinitionExecuter
Executes the database creation sql commands from the DataDefinitionGenerator, to create the database.
### DbTools.DataDefinitionGenerator
Generates executable sql to create database from DatabaseDefinition.
### DbTools.DataDefinitionReader
Builds DatabaseDefinition from an existing database.
### DbTools.DataGenerator
Generates data, typically for testing purposes, based on DatabaseDefinition.
### DbTools.TestBase
Contains helper classes for unit testing DbTools.

## Development setup
### Config files
Configuration values are stored in config.json files.
Configuration values for tests are stored in testconfig.json files.

You can use local config values to override content of these files, which are local (developer specific), or not desired to be included in source control (for example, a connection string with username and password to an internal environment).
Use naming as config-local.json and testconfig-local.json.

### Running Unit tests
To run unit tests in parallel between assemblies, you have to enable it by selecting Test Explorer / Configure Run Settings / Select solution wide runsettings file, and then select the file \Tests\test.runsettings

See https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?branch=release-16.4&view=vs-2022