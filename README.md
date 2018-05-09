# DotNet.Property

.NET Core command-line (CLI) tool to update project properties and version numbers on build.

## Usage

Install the global tool

    dotnet tool install -g dotnet-property

Update project property

    dotnet property <projectGlob> <property:value>

| Argument  | Description |
| ------------- | ------------- |
| `projectGlob` | Path glob expression of projects to update.  Must be first argument.  |
| `property:value`  | Project property name value pair. Can have multiple properties to update.  |

### Examples

Set the version number in a shared props file

    dotnet property "**/version.props" Version:"1.0.0.3"

Update project version and copyright properties

    dotnet property "**/Project.csproj" Version:"1.0.0.3" Copyright:"Copyright 2018 LoreSoft"
