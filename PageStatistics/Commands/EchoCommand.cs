using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Logging;

namespace PageStatistics.Commands
{
    public class EchoCommand : Command
    {
        private const string CommandName = "echo";
        private const string CommandDescription = "Print passed string";

        private readonly ILogger<EchoCommand> _logger;

        public EchoCommand(ILogger<EchoCommand> logger)
            : base(CommandName, CommandDescription)
        {
            _logger = logger;

            ConfigureCommand();
        }

        private void ConfigureCommand()
        {
            var echoArgument = new Argument<string>
            {
                Name = "echo"
            };

            AddArgument(echoArgument);

            Handler = CommandHandler.Create(
                (Func<string, int>) HandleCommand);
        }

        private int HandleCommand(string echo)
        {
            _logger.Log(LogLevel.Information, "App is running");

            Console.WriteLine($"{echo}");

            return 1;
        }
    }
}
