using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Handlers.Keyboards;
using BotRiveGosh.Services.Interfaces;
using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Views.ToDo
{
    public class TodoMenuView : BaseView
    {
        private readonly ITodoService _todoService;
        private readonly IUserService _userService;

        public TodoMenuView(ITelegramBotClient botClient, ITodoService todoService,
            IUserService userService) : base(botClient)
        {
            _todoService = todoService;
            _userService = userService;
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);
            CallBackDto dto = new CallBackDto("");
            if (Text != null)
            {
                dto = CallBackDto.FromString(Text);
                if(update.CallbackQuery != null)
                {
                    await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                    await _botClient.EditMessageText(ChatId, MessageId, "Мои задачи.",
                        replyMarkup: await GetKeybords(dto, ct),
                        cancellationToken: ct);
                }
            }
                
        }

        private async Task<InlineKeyboardMarkup?> GetKeybords(CallBackDto dto, CancellationToken ct)
        {
            var user = await _userService.GetUserByTelegramIdAsync(ChatId, ct);

            if(user != null)
            {
                //все незавершенные задачи
                var todos = await _todoService.GetActiveByUserid(user.Id, ct);

                //задачи на сегодня
                var todosToday = todos
                    .Where(t => t.FinishedAt == DateOnly.FromDateTime(DateTime.UtcNow))
                    .ToList();

                //просроченные задачи
                var todosOverdue = todos
                    .Where(t => t.FinishedAt < DateOnly.FromDateTime(DateTime.UtcNow))
                    .ToList();

                var buttons = new List<List<InlineKeyboardButton>>();

                //все задачи
                if (todos != null && todos.Count() > 0)
                {
                    buttons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData("все задачи",new CallBackDto(Dto_Objects.TodoListView, Dto_Action.Show).ToString())
                    });
                }

                //задачи на сегодня, делаем кнопку "Задачи на сегодня"
                if (todosToday != null && todosToday.Count() > 0)
                {
                    buttons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData("⚡ задачи на сегодня",new CallBackDto(Dto_Objects.TodoListView, Dto_Action.ShowToday).ToString())
                    });
                }

                //просроченные задачи
                if (todosOverdue != null && todosOverdue.Count() > 0)
                {
                    buttons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData("⚠️ просроченные задачи",new CallBackDto(Dto_Objects.TodoListView, Dto_Action.ShowOverdue).ToString())
                    });
                }

                //кнопка НАЗАД и ДОБАВИТЬ
                buttons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ назад ",new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.Show).ToString()),
                        InlineKeyboardButton.WithCallbackData("➕",new CallBackDto(Dto_Objects.Todo, Dto_Action.AddTodoScenario).ToString())
                    });

                return new InlineKeyboardMarkup(buttons);
            }

            return null;
        }
    }
}
