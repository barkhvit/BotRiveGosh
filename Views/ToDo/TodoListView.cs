using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Services.Interfaces;
using LinqToDB.Common;
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
    public class TodoListView : BaseView
    {
        private readonly IUserService _userService;
        private readonly ITodoService _todoService;
        public TodoListView(ITelegramBotClient botClient, ITodoService todoService,
            IUserService userService) : base(botClient)
        {
            _todoService = todoService;
            _userService = userService;
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);

            //получаем пользователя
            var user = await _userService.GetUserByTelegramIdAsync(ChatId, ct);

            //получаем dto
            if(Text != null)
            {
                var dto = CallBackDto.FromString(Text);

                //в зависимости от dto.Action получаем TypeOfTodoList
                TypeOfTodoList typeOfTodoList = dto.Action switch
                {
                    nameof(Dto_Action.Show) => TypeOfTodoList.todoAll,
                    nameof(Dto_Action.ShowOverdue) => TypeOfTodoList.todoOverdue,
                    nameof(Dto_Action.ShowToday) => TypeOfTodoList.todoToday,
                    _ => throw new Exception("нет типа в методе TodoDoneView.Show")
                };

                //получаем задачи пользователя в зависимости от typeOfTodoList и отправляем сообщение
                if (user != null)
                {
                    var todos = await _todoService.GetActiveByUserid(user.Id, ct);

                    if(typeOfTodoList == TypeOfTodoList.todoOverdue)
                    {
                        todos = todos.Where(t => t.FinishedAt < DateOnly.FromDateTime(DateTime.UtcNow)).ToList();
                    }

                    if (typeOfTodoList == TypeOfTodoList.todoToday)
                    {
                        todos = todos.Where(t => t.FinishedAt == DateOnly.FromDateTime(DateTime.UtcNow)).ToList();
                    }

                    if (todos != null)
                    {
                        await SendMessage(todos, update, typeOfTodoList, ct);
                    }
                }
            }
        }

        //Отправка сообщения
        private async Task SendMessage(IReadOnlyList<Todo> todos, Update update, TypeOfTodoList typeOfTodoList, CancellationToken ct)
        {
            string text = typeOfTodoList switch 
            {
                TypeOfTodoList.todoAll => "Все задачи:\n",
                TypeOfTodoList.todoOverdue => "Просроченные задачи:\n",
                TypeOfTodoList.todoToday => "Задачи на сегодня:\n",
                _ => ""
            };

            foreach(var t in todos)
            {
                string point = t.FinishedAt < DateOnly.FromDateTime(DateTime.UtcNow) ? "🔴" : "🟢";
                if (t.FinishedAt == DateOnly.FromDateTime(DateTime.UtcNow)) point = "🟡";
                text += $"{point}{t.Name} до {t.FinishedAt.ToString("dd.MM.yyyy")}\n";
            }
            if (update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            await _botClient.EditMessageText(ChatId, MessageId, text, cancellationToken: ct, replyMarkup: GetKeyboard(typeOfTodoList));
        }

        //клавиатура
        private InlineKeyboardMarkup GetKeyboard(TypeOfTodoList typeOfTodoList)
        {
            var buttons = new List<List<InlineKeyboardButton>>();

            //в зависимости от typeOfTodoList делаем dto для кнопки редатировать
            var dtoEdit = typeOfTodoList switch
            {
                TypeOfTodoList.todoAll => new CallBackDto(Dto_Objects.TodoEditView, Dto_Action.Show).ToString(),
                TypeOfTodoList.todoOverdue => new CallBackDto(Dto_Objects.TodoEditView, Dto_Action.ShowOverdue).ToString(),
                TypeOfTodoList.todoToday => new CallBackDto(Dto_Objects.TodoEditView, Dto_Action.ShowToday).ToString(),
                _ => throw new ArgumentException("Нет типа для typeOfTodoList в методе TodoListView.GetKeyboard")
            };

            buttons.Add(new List<InlineKeyboardButton>()
                {
                InlineKeyboardButton.WithCallbackData("⬅️ назад",new CallBackDto(Dto_Objects.TodoMenuView,Dto_Action.Show).ToString()),
                InlineKeyboardButton.WithCallbackData("Редактировать ", dtoEdit)
                });

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
