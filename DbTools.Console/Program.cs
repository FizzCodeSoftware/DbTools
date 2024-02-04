using System.Linq;
using System.Text.RegularExpressions;
using CommandDotNet;
using CommandDotNet.Help;
using FizzCode.LightWeight.Configuration;
using Microsoft.Extensions.Configuration;

namespace FizzCode.DbTools.Console;
public static class Program
{
    public static bool Terminated { get; set; }

    public static IConfigurationRoot Configuration { get; private set; }

    public static void Main(string[] args)
    {
        Configuration = ConfigurationLoader.LoadFromJsonFile("config");
        DbProviderFactoryRegistrator.LoadFromConfiguration(Configuration);

        if (args.Length > 0)
        {
            var runner = new AppRunner<AppCommands>(GetAppSettings());
            runner.Run(args);
        }

        DisplayHelp();

        var regEx = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+");

        while (!Terminated)
        {
            System.Console.Write("> ");
            var commandLine = System.Console.ReadLine();
            if (string.IsNullOrEmpty(commandLine))
                continue;

            var lineArguments = regEx
                .Matches(commandLine.Trim())
                .Select(x => x.Value.Trim())
                .ToArray();

            var runner = new AppRunner<AppCommands>(GetAppSettings());
            runner.Run(lineArguments);

            System.Console.WriteLine();
        }
    }

    internal static void DisplayHelp(string command = null)
    {
        var runner = new AppRunner<AppCommands>(GetAppSettings());

        if (string.IsNullOrEmpty(command))
        {
            runner.Run("--help");
        }
        else
        {
            var args = command.Split(' ').ToList();
            args.Add("--help");
            runner.Run(args.ToArray());
        }
    }

    private static AppSettings GetAppSettings()
    {
        return new AppSettings()
        {
            Help = new AppHelpSettings()
            {
                TextStyle = HelpTextStyle.Basic,
                UsageAppName = ">",
                PrintHelpOption = false,
            },
        };
    }
}
