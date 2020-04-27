using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace DotNet.Property
{
    /// <summary>
    /// Update .net core project properties
    /// </summary>
    public class ProjectUpdater
    {

        /// <summary>
        /// Gets or sets the log writer delegate.
        /// </summary>
        /// <value>
        /// The log writer delegate.
        /// </value>
        public Action<string> Logger { get; set; } = Console.WriteLine;

        /// <summary>
        /// Gets or sets the properties to update.
        /// </summary>
        /// <value>
        /// The properties to update.
        /// </value>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the projects to update as a file glob expression.
        /// </summary>
        /// <value>
        /// The projects to update as a file glob expression.
        /// </value>
        public string Projects { get; set; } = "**/*.props";


        /// <summary>
        /// Updates .net core projects using the specified command line arguments. The first argument is the project list glob expression.
        /// </summary>
        /// <param name="args">The arguments used to update project with.</param>
        /// <param name="rootDirectory">The root directory to search for projects.</param>
        public void Update(string[] args, string rootDirectory = null)
        {
            Projects = args[0];
            Properties = ParseArguments(args, 1);

            WriteLog($"Matching projects: {Projects}");
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude(Projects);

            if (string.IsNullOrEmpty(rootDirectory))
                rootDirectory = Environment.CurrentDirectory;

            var currentDirectory = new DirectoryInfo(rootDirectory);
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
                try{
                    UpdateProject(filePath);
                }
                catch(Exception ex){ // intercept error and try to process next file
                    Console.WriteLine($"error occured processing {filePath} : {ex.Message}");
                }
                    
            }
        }


        /// <summary>
        /// Updates the project at the specified <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path of the project to update.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is not found.</exception>
        public void UpdateProject(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new ArgumentException($"File not found: {filePath}", nameof(filePath));

            WriteLog($"Updating Project: {filePath}");

            XDocument document;
            using (var readStream = File.OpenRead(filePath))
            {
                document = XDocument.Load(readStream);
            }

            UpdateProject(document);

            // save document
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = XmlWriter.Create(stream, settings))
            {
                document.Save(writer);
            }
        }

        /// <summary>
        /// Updates the project as an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="document">The project document.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Missing root Project node.</exception>
        public void UpdateProject(XDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (Properties == null || Properties.Count == 0)
                return;

            var projectElement = document.Element("Project");
            if (projectElement == null)
                throw new InvalidOperationException("Could not find root Project node. Make sure file has a root Project node without a namespace.");

            foreach (var p in Properties)
            {
                WriteLog($"  Set Property '{p.Key}':'{p.Value}'");
                
                // find last group with element and no condition
                var projectGroup = projectElement
                    .Elements("PropertyGroup")
                    .LastOrDefault(e => 
                        e.Elements(p.Key).Any() && 
                        (e.HasAttributes == false || e.Attributes().All(a => a.Name != "Condition"))
                    );

                // use last group without condition
                if (projectGroup == null)
                    projectGroup = projectElement
                        .Elements("PropertyGroup")
                        .LastOrDefault(e => e.HasAttributes == false || e.Attributes().All(a => a.Name != "Condition"));

                // create new if not found
                if (projectGroup == null)
                    projectGroup = projectElement
                        .GetOrCreateElement("PropertyGroup");

                projectGroup
                    .GetOrCreateElement(p.Key)
                    .SetValue(p.Value);
            }
        }


        /// <summary>
        /// Parses the arguments into a dictionary.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is <see langword="null"/></exception>
        public static Dictionary<string, string> ParseArguments(string[] args, int startIndex = 0)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

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
