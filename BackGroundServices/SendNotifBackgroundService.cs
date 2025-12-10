using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

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
                            //в зависимости от темы делаем кнопку, по умолчанию только главное меню
                            List<InlineKeyboardButton> firstRow = new List<InlineKeyboardButton>()
                            {
                                InlineKeyboardButton.WithCallbackData("🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenuNewMessage).ToString())
                            };

                            if (n.Subject.Contains("todayTodo"))
                            {
                                firstRow.Add(InlineKeyboardButton.WithCallbackData("Подробнее..", new CallBackDto(Dto_Objects.TodoEditView, Dto_Action.ShowToday).ToString()));
                            }

                            if (n.Subject.Contains("overdueTodо"))
                            {
                                firstRow.Add(InlineKeyboardButton.WithCallbackData("Подробнее..", new CallBackDto(Dto_Objects.TodoEditView, Dto_Action.ShowOverdue).ToString()));
                            }

                            var buttons = new List<List<InlineKeyboardButton>>()
                            {
                                firstRow
                            };

                            //отправляем сообщение пользователю
                            await _botClient.SendMessage(n.User.TelegramId, n.Text, cancellationToken: ct,
                                replyMarkup: new InlineKeyboardMarkup(buttons));

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
