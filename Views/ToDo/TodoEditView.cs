using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Services.Interfaces;
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
    public class TodoEditView : BaseView
    {
        private readonly IUserService _userService;
        private readonly ITodoService _todoService;
        public TodoEditView(ITelegramBotClient botClient, ITodoService todoService,
            IUserService userService) : base(botClient)
        {
            _todoService = todoService;
            _userService = userService;
        }

        //редактировать все задачи
        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);

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

                //Получаем пользователя
                var user = await _userService.GetUserByTelegramIdAsync(ChatId, ct);

                //получаем список задач и отправляем сообщение
                if (user != null)
                {
                    //все задачи пользователя
                    var todos = await _todoService.GetActiveByUserid(user.Id, ct);

                    //задачи просроченные
                    if (typeOfTodoList == TypeOfTodoList.todoOverdue)
                    {
                        todos = todos.Where(t => t.FinishedAt < DateOnly.FromDateTime(DateTime.UtcNow)).ToList();
                    }

                    //задачи на сегодня
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

        //отправка сообщений
        private async Task SendMessage(IReadOnlyList<Todo> todos, Update update, TypeOfTodoList typeOfTodoList, CancellationToken ct)
        {
            string text = "Выберите задачу для просмотра/редактирования:";

            if (update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

            await _botClient.EditMessageText(ChatId, MessageId, text, cancellationToken: ct, 
                replyMarkup: GetKeyboards(todos, typeOfTodoList));
        }

        private InlineKeyboardMarkup GetKeyboards(IReadOnlyList<Todo> todos, TypeOfTodoList typeOfTodoList)
        {
            var buttons = new List<List<InlineKeyboardButton>>();

            //в зависимости от typeOfTodoList делаем dto для кнопки НАЗАД
            var backButtonDto = typeOfTodoList switch
            {
                TypeOfTodoList.todoAll => new CallBackDto(Dto_Objects.TodoListView, Dto_Action.Show).ToString(),
                TypeOfTodoList.todoOverdue => new CallBackDto(Dto_Objects.TodoListView, Dto_Action.ShowOverdue).ToString(),
                TypeOfTodoList.todoToday => new CallBackDto(Dto_Objects.TodoListView, Dto_Action.ShowToday).ToString(),
                _ => throw new Exception("Неизвестный тип TypeOfTodoList в методе TodoEditView.GetKeyboards")
            };

            //добавляем кнопки с задачами
            foreach(var t in todos)
            {
                string DtoAction = typeOfTodoList switch
                {
                    //в зависимости от typeOfTodoList делаем dto для кнопки с названием задачи
                    TypeOfTodoList.todoAll => Dto_Action.Show,
                    TypeOfTodoList.todoOverdue => Dto_Action.ShowOverdue,
                    TypeOfTodoList.todoToday => Dto_Action.ShowToday,
                    _ => throw new Exception("Неизвестный тип TypeOfTodoList в методе TodoEditView.GetKeyboards")
                };
                buttons.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData($"{t.Name}",new CallBackDto(
                        Dto_Objects.TodoDetailView, DtoAction, _id: t.Id).ToString())
                });
            }

            //Добавляем кнопку НАЗАД
            buttons.Add(new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("⬅️ назад",backButtonDto)
            });

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
