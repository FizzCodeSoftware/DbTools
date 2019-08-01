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
### .config files
You can use local config values to override content of App.config files, which are local (developer specific), or not desired to be included in source control (for example, a connection string with username and password to an internal environment).

Project specific:
App.Local.config

Global:
\Global\App.Local.Global.config 

using TransformXml build task.

Typically test projects (integration tests, using database to create or read for testing purposes) have App.config files with connections and developers can have their own overrides in App.Local.config or App.Local.Global.config.

The forceIntegrationTests config value conrtrols whether database test, other than in memory SqLite version should run. This config value is typically overridden in the App.Local.Global.config file.

\Global\App.Local.Global.config example:

<?xml version="1.0" encoding="utf-8"?>
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
  <appSettings>
    <add key="forceIntegrationTests" value="true" xdt:Transform="Replace" xdt:Locator="Match(key)"/>
  </appSettings>
</configuration>