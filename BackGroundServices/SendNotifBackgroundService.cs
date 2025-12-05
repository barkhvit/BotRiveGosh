using BotRiveGosh.Services.Interfaces;
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
    class SendNotifBackgroundService : BackgroundService
    {
        private readonly ILogger<SendNotifBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationService _notificationService;
        private readonly ITelegramBotClient _botClient;

        public SendNotifBackgroundService(ILogger<SendNotifBackgroundService> logger, IServiceProvider serviceProvider,
            INotificationService notificationService, ITelegramBotClient botClient)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Ну вот я запустился, хули надо");

                // Ждем сигнала остановки
                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(3000, ct);

                    //получаем нотификации
                    var notificationForSending = await _notificationService.GetForSentingAsync(ct);

                    if(notificationForSending.Count > 0)
                    {
                        foreach(var n in notificationForSending)
                        {
                            //отправляем сообщение пользователю
                            await _botClient.SendMessage(n.User.TelegramId, n.Text, cancellationToken: ct);

                            //делаем сообщение отправленным
                            await _notificationService.MarkAsSentAsync(n.Id, ct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting SendNotifBackgroundService");
                throw;
            }
        }
    }
}
