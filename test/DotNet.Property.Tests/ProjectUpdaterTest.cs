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
        public void UpdateProject()
        {
            var properties = new Dictionary<string, string>
            {
                { "Version", "2.0.0.0" },
                { "Copyright", "Copyright 2018 LoreSoft" }
            };

            var xml = @"<Project>
  <PropertyGroup>
    <Version>1.0.0.0</Version>
  </PropertyGroup>
</Project>";

            var document = XDocument.Parse(xml);


            var updater = new ProjectUpdater();
            updater.Properties = properties;
            updater.UpdateProject(document);
        }
    }
}
