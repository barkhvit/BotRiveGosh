using BotRiveGosh.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BotRiveGosh
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly ILogger<BotBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cts;

        public BotBackgroundService(ILogger<BotBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cts = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bot service starting...");

            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
                    var mainHandler = scope.ServiceProvider.GetRequiredService<MainHandler>();

                    // Запускаем прослушку бота
                    botClient.StartReceiving(
                        updateHandler: mainHandler.HandleUpdateAsync,
                        errorHandler: mainHandler.HandleErrorAsync,
                        cancellationToken: _cts.Token);

                    _logger.LogInformation("Bot started successfully");

                    // Ждем сигнала остановки
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error starting bot service");
                    throw;
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot service stopping...");
            _cts.Cancel();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Bot service stopped");
        }
    }
}
