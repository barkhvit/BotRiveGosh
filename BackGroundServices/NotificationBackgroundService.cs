using BotRiveGosh.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BotRiveGosh.BackGroundServices
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cts;
        private readonly TimeSpan _timeSpan;

        public NotificationBackgroundService(ILogger<NotificationBackgroundService> logger, IServiceProvider serviceProvider,
            TimeSpan? timeSpan = null)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cts = new CancellationTokenSource();
            _timeSpan = TimeSpan.FromMinutes(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    _logger.LogInformation("NotificationService started");

                    // Ждем сигнала остановки
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(_timeSpan, stoppingToken);
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
