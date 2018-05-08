using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace DotNet.Property
{
    public class ProjectUpdater
    {
        public Action<string> Logger { get; set; } = Console.WriteLine;

        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string Projects { get; set; } = "**/*.props";


        public void Update(string[] args)
        {
            Projects = args[0];
            Properties = ParseArguments(args, 1);

            WriteLog($"Matching projects: {Projects}");
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude(Projects);

            var currentDirectory = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
            var startingDirectory = new DirectoryInfoWrapper(currentDirectory);
            var matchingResult = matcher.Execute(startingDirectory);

            if (!matchingResult.HasMatches)
            {
                WriteLog($"Error: No Projects found: {Projects}");
                return;
            }

            foreach (var fileMatch in matchingResult.Files)
            {
                var filePath = Path.GetFullPath(fileMatch.Path);
                UpdateProject(filePath);
            }
        }


        public void UpdateProject(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new ArgumentException($"File not found: {filePath}", nameof(filePath));

            WriteLog($"Updating Project: {filePath}");
            var document = XDocument.Load(filePath);

            UpdateProject(document);
        }

        public void UpdateProject(XDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (Properties == null || Properties.Count == 0)
                return;

            foreach (var p in Properties)
            {
                WriteLog($"  Set Property '{p.Key}':'{p.Value}'");

                document
                    .GetOrCreateElement("Project")
                    .GetOrCreateElement("PropertyGroup")
                    .GetOrCreateElement(p.Key)
                    .SetValue(p.Value);
            }
        }


        public static Dictionary<string, string> ParseArguments(string[] args, int startIndex = 0)
        {
            var arguments = new Dictionary<string, string>();

            // skip first
            for (int i = startIndex; i < args.Length; i++)
            {
                var parts = args[i].Split(':', '=');
                var name = parts[0];
                var value = parts.Length > 1 ? parts[1] : string.Empty;

                arguments[name] = value;
            }

            return arguments;
        }


        private void WriteLog(string s)
        {
            if (Logger == null || string.IsNullOrEmpty(s))
                return;

            Logger.Invoke(s);
        }
    }
}
