using BotRiveGosh.Core.Entities;
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
    public class TodoNotificationBackgroundService : BackgroundService
    {
        private const string strToday = "todayTodo"; //для сегодняшних дел
        private const string strOverdue = "overdueTodо"; //для просроченных дел

        private readonly INotificationService _notificationService;
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoNotificationBackgroundService> _logger;
        private readonly ITelegramBotClient _botClient;

        public TodoNotificationBackgroundService(
            INotificationService notificationService,
            ITodoService todoService,
            ILogger<TodoNotificationBackgroundService> logger,
            ITelegramBotClient botClient
            )
        {
            _notificationService = notificationService;
            _todoService = todoService;
            _logger = logger;
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Запустился фоновый сервис: TodoNotificationBackgroundService.ExecuteAsync");

                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), ct);

                    //Получаем задачи на сегодня и создаем уведомление
                    var todos = await _todoService.GetActiveByFinishedDateAsync(DateOnly.FromDateTime(DateTime.UtcNow), ct);
                    await CreateNotificationAsync(todos, new DateTime(DateOnly.FromDateTime(DateTime.UtcNow), new TimeOnly(9, 30)), strToday, ct);

                    //Получаем просроченные задачи и создаем уведомление
                    var todosOverdue = await _todoService.GetActiveAndOverdueAsync(ct);
                    await CreateNotificationAsync(todosOverdue, new DateTime(DateOnly.FromDateTime(DateTime.UtcNow), new TimeOnly(9, 35)), strOverdue, ct);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка в TodoNotificationBackgroundService.ExecuteAsync");
                throw;
            }
        }


        private async Task CreateNotificationAsync(IReadOnlyList<Todo> todos, DateTime sentTo, string subjectType, CancellationToken ct)
        {
            if (todos.Count > 0)
            {
                //получаем пользователей
                var users = todos
                    .Select(t => t.User)
                    .Distinct()
                    .ToList();

                //для каждого пользователя
                foreach (var u in users)
                {
                    //проверяем наличие уведомления
                    string subject = $"{u.TelegramId}_{subjectType}_{DateTime.UtcNow.ToString("dd.MM.yyyy")}";
                    var notification = await _notificationService.GetNotificationBySubject(subject, ct);

                    if (notification == null)
                    {
                        //создаем нотификацию

                        //задачи для каждого пользователя
                        var todosForUser = todos
                            .Where(t => t.User.Id == u.Id)
                            .ToList();

                        //формируем начало сообщения
                        string text = subjectType switch
                        {
                            strToday => "Просто напоминаю, что у вас есть задачи на сегодня:",
                            strOverdue => "Просто напоминаю, что у вас есть просроченные задачи:",
                            _ => ""
                        };

                        //формируем текст
                        foreach (var t in todosForUser)
                            text += $"\n⚡ {t.Name} до {t.FinishedAt.ToString("dd.MM.yyyy")}";

                        //создаем уведомление
                        var newNotification = new Notification()
                        {
                            Id = Guid.NewGuid(),
                            User = u,
                            Subject = subject,
                            Text = text,
                            SendTo = sentTo,
                            Sent = false
                        };

                        //добавляем уведомление
                        var nNotif = await _notificationService.AddAsync(newNotification, ct);

                        //если уведомление добавлено отправляем инфо
                        if (nNotif > 0)
                            _logger.LogInformation($"Добавлено уведомление с темой: {newNotification.Subject}");
                    }
                }
            }
        }
    }
}
