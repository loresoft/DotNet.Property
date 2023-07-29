using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Property.Tests
{
    public class ProjectUpdaterTest
    {
        public ProjectUpdaterTest(ITestOutputHelper output)
        {
            Output = output;
        }

        public ITestOutputHelper Output { get; }

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
        public void ParseArgumentsUrl()
        {
            var args = ProjectUpdater.ParseArguments(new[]
            {
                "Version=1.0.0.3",
                "PackageProjectUrl=https://test.com",
                "SourceProjectUrl=https://other.com"
            }, 0);

            args.Should().NotBeNull();
            args.Count.Should().Be(3);
            args["Version"].Should().Be("1.0.0.3");
            args["PackageProjectUrl"].Should().Be("https://test.com");
            args["SourceProjectUrl"].Should().Be("https://other.com");
        }

        [Fact]
        public void UpdateProject()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Copyright", "Copyright 2018 LoreSoft" }
            };

            var xml = "<Project><PropertyGroup><Version>1.0.0.0</Version></PropertyGroup></Project>";
            var document = XDocument.Parse(xml);


            var updater = new ProjectUpdater();
            updater.Properties = properties;
            updater.Logger = Output.WriteLine;

            updater.UpdateProject(document);

            var result = document.ToString(SaveOptions.DisableFormatting);

            var expected = "<Project><PropertyGroup><Version>2.0.0.0</Version><Copyright>Copyright 2018 LoreSoft</Copyright></PropertyGroup></Project>";
            result.Should().Be(expected);
        }

        [Fact]
        public void UpdateProjectMissingGroup()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Copyright", "Copyright 2018 LoreSoft" }
            };

            var xml = "<Project></Project>";
            var document = XDocument.Parse(xml);


            var updater = new ProjectUpdater();
            updater.Properties = properties;
            updater.Logger = Output.WriteLine;

            updater.UpdateProject(document);

            var result = document.ToString(SaveOptions.DisableFormatting);

            var expected = "<Project><PropertyGroup><Version>2.0.0.0</Version><Copyright>Copyright 2018 LoreSoft</Copyright></PropertyGroup></Project>";
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

            var xml = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFrameworks>net45;netstandard2.0</TargetFrameworks></PropertyGroup><PropertyGroup><Description>Test</Description></PropertyGroup><PropertyGroup><Product>dotnet-property</Product></PropertyGroup><PropertyGroup Condition=\" \'$(TargetFramework)\' == \'netstandard2.0\' \"><Description>Test (.NET Core 2.0) </Description></PropertyGroup></Project>";
            var document = XDocument.Parse(xml);


            var updater = new ProjectUpdater();
            updater.Properties = properties;
            updater.Logger = Output.WriteLine;

            updater.UpdateProject(document);

            var result = document.ToString(SaveOptions.DisableFormatting);

            var expected = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFrameworks>net45;netstandard2.0</TargetFrameworks></PropertyGroup><PropertyGroup><Description>Nested Version</Description></PropertyGroup><PropertyGroup><Product>dotnet-property</Product><Version>2.0.0.0</Version></PropertyGroup><PropertyGroup Condition=\" \'$(TargetFramework)\' == \'netstandard2.0\' \"><Description>Test (.NET Core 2.0) </Description></PropertyGroup></Project>";
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

            var projectPath = Path.Combine(Environment.CurrentDirectory, "samples", "SampleLibrary.xml");

            var updater = new ProjectUpdater();
            updater.Properties = properties;
            updater.Logger = Output.WriteLine;

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

            var projectPath = Path.Combine(Environment.CurrentDirectory, "samples", "NestedGroup.xml");

            var updater = new ProjectUpdater();
            updater.Properties = properties;
            updater.Logger = Output.WriteLine;

            updater.UpdateProject(projectPath);
        }
    }
}
