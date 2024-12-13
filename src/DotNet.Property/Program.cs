namespace DotNet.Property;

public static class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            OutputVersion();
            OutputUsage();

            return 1;
        }

        try
        {
            var updater = new ProjectUpdater();
            updater.Update(args, Environment.CurrentDirectory);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }

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
