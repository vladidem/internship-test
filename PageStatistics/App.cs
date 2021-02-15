using Microsoft.Extensions.Logging;

namespace PageStatistics
{
    public class App
    {
        private readonly ILogger<App> _logger;

        public App(ILogger<App> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.Log(LogLevel.Information, "App is running");
        }
    }
}
