using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace DotNet.Property
{
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                OutputVersion();
                OutputUsage();

                return 1;
            }

            var updater = new ProjectUpdater();
            updater.Update(args);

            return 0;
        }

        private static void OutputVersion()
        {
            Console.WriteLine(".NET Core command-line (CLI) tool to update project properties");
        }

        private static void OutputUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: dotnet property <project> <property>:<value>");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  dotnet property \"**/version.props\" Version:\"1.0.0.3\" Copyright:\"Copyright 2018 LoreSoft\"");
        }
    }
}
