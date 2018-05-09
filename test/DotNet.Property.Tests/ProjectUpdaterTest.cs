using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace DotNet.Property.Tests
{
    public class ProjectUpdaterTest
    {
        [Fact]
        public void ParseArguments()
        {
            var args = ProjectUpdater.ParseArguments(new[]
            {
                "Version:1.0.0.3",
                "Copyright:Copyright 2018 LoreSoft"
            }, 0);

            args.Should().NotBeNull();
            args.Count.Should().Be(2);
            args["Version"].Should().Be("1.0.0.3");
            args["Copyright"].Should().Be("Copyright 2018 LoreSoft");
        }

        [Fact]
        public void ParseArgumentsEquals()
        {
            var args = ProjectUpdater.ParseArguments(new[]
            {
                "Version=1.0.0.3",
                "Copyright=Copyright 2018 LoreSoft"
            }, 0);

            args.Should().NotBeNull();
            args.Count.Should().Be(2);
            args["Version"].Should().Be("1.0.0.3");
            args["Copyright"].Should().Be("Copyright 2018 LoreSoft");
        }

        [Fact]
        public void UpdateProject()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Copyright", "Copyright 2018 LoreSoft" }
            };

            var xml = "<Project>\r\n  <PropertyGroup>\r\n    <Version>1.0.0.0</Version>\r\n  </PropertyGroup>\r\n</Project>";
            var document = XDocument.Parse(xml);


            var updater = new ProjectUpdater();
            updater.Properties = properties;

            updater.UpdateProject(document);

            var result = document.ToString();

            var expected = "<Project>\r\n  <PropertyGroup>\r\n    <Version>2.0.0.0</Version>\r\n    <Copyright>Copyright 2018 LoreSoft</Copyright>\r\n  </PropertyGroup>\r\n</Project>";
            result.Should().Be(expected);
        }

        [Fact]
        public void UpdateProjectComplex()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Description", "Nested Version" }
            };

            var xml = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n  <PropertyGroup>\r\n    <TargetFrameworks>net45;netstandard2.0</TargetFrameworks>\r\n  </PropertyGroup>\r\n  <PropertyGroup>\r\n    <Description>Test</Description>\r\n  </PropertyGroup>\r\n  <PropertyGroup>\r\n    <Product>dotnet-property</Product>\r\n  </PropertyGroup>\r\n  <PropertyGroup Condition=\" \'$(TargetFramework)\' == \'netstandard2.0\' \">\r\n    <Description>Test (.NET Core 2.0) </Description>\r\n  </PropertyGroup>\r\n</Project>";
            var document = XDocument.Parse(xml);


            var updater = new ProjectUpdater();
            updater.Properties = properties;

            updater.UpdateProject(document);

            var result = document.ToString();

            var expected = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n  <PropertyGroup>\r\n    <TargetFrameworks>net45;netstandard2.0</TargetFrameworks>\r\n  </PropertyGroup>\r\n  <PropertyGroup>\r\n    <Description>Nested Version</Description>\r\n  </PropertyGroup>\r\n  <PropertyGroup>\r\n    <Product>dotnet-property</Product>\r\n    <Version>2.0.0.0</Version>\r\n  </PropertyGroup>\r\n  <PropertyGroup Condition=\" \'$(TargetFramework)\' == \'netstandard2.0\' \">\r\n    <Description>Test (.NET Core 2.0) </Description>\r\n  </PropertyGroup>\r\n</Project>";
            result.Should().Be(expected);
        }


        [Fact]
        public void UpdateProjectSampleLibrary()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Copyright", "Copyright 2018 LoreSoft" }
            };

            var projectPath = @"samples\SampleLibrary.xml";


            var updater = new ProjectUpdater();
            updater.Properties = properties;

            updater.UpdateProject(projectPath);
        }


        [Fact]
        public void UpdateProjectNestedGroup()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Description", "Nested Version" }
            };

            var projectPath = @"samples\NestedGroup.xml";


            var updater = new ProjectUpdater();
            updater.Properties = properties;

            updater.UpdateProject(projectPath);
        }
    }
}
