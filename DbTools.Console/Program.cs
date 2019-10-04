namespace FizzCode.DbTools.Console
{
    using System;
    using System.Linq;
    using CommandDotNet;
    using CommandDotNet.Models;
    using FizzCode.DbTools.Configuration;
    using Microsoft.Extensions.Configuration;

    public static class Program
    {
        public static bool Terminated { get; set; }

        public static IConfigurationRoot Configuration { get; private set; }

        public static void Main(string[] args)
        {
            Configuration = Common.Configuration.Load("config");
            DbProviderFactoryRegistrator.LoadFromConfiguration(Configuration);

            if (args.Length > 0)
            {
                var runner = new AppRunner<AppCommands>(GetAppSettings());
                runner.Run(args);
            }

            DisplayHelp();

            while (!Terminated)
            {
                Console.Write("> ");
                var commandLine = Console.ReadLine();
                if (string.IsNullOrEmpty(commandLine))
                    continue;

                var lineArguments = commandLine.Split(' ');
                var runner = new AppRunner<AppCommands>(GetAppSettings());
                runner.Run(lineArguments);

                Console.WriteLine();
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
                EnableVersionOption = false,
                Case = Case.KebabCase,
                Help = new AppHelpSettings()
                {
                    TextStyle = HelpTextStyle.Basic,
                    UsageAppNameStyle = UsageAppNameStyle.GlobalTool,
                },
            };
        }
    }
}
