using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PageStatistics.Services;

namespace PageStatistics.Commands
{
    public class CollectStatisticsCommand : Command
    {
        private const string CommandName = "collect";
        private const string CommandDescription = "Download page by address and collect word statistics";

        private readonly IPageLoader _loader;
        private readonly ILogger<EchoCommand> _logger;

        public CollectStatisticsCommand(ILogger<EchoCommand> logger, IPageLoader loader)
            : base(CommandName, CommandDescription)
        {
            _logger = logger;
            _loader = loader;

            ConfigureCommand();
        }

        private void ConfigureCommand()
        {
            var addressArgument = new Argument<string>
            {
                Name = "address"
            };

            AddArgument(addressArgument);

            Handler = CommandHandler.Create(
                (Func<string, Task<int>>) HandleCommand);
        }

        private async Task<int> HandleCommand(string address)
        {
            _logger.Log(LogLevel.Information, $"Started downloading page {address}");
            var fileName = await _loader.Download(address);
            _logger.Log(LogLevel.Information, $"Page stored at {fileName}");

            return 1;
        }
    }
}
