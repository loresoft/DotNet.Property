# DotNet.Property

.NET Core command-line (CLI) tool to update project properties and version numbers on build.

[![Build Project](https://github.com/loresoft/DotNet.Property/actions/workflows/dotnet.yml/badge.svg)](https://github.com/loresoft/DotNet.Property/actions/workflows/dotnet.yml)

[![Coverage Status](https://coveralls.io/repos/github/loresoft/DotNet.Property/badge.svg)](https://coveralls.io/github/loresoft/DotNet.Property)

[![NuGet Version](https://img.shields.io/nuget/v/dotnet-property.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-property/)

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
