using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Handlers.Keyboards;
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
    public class TodoDetailView : BaseView
    {
        private readonly ITodoService _todoService;
        public TodoDetailView(ITelegramBotClient botClient, ITodoService todoService) : base(botClient)
        {
            _todoService = todoService;
        }

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

                //получаем задачу
                if (dto.Id != null)
                {
                    var todo = await _todoService.GetTodoById((Guid)dto.Id, ct);

                    //отправляем сообщение
                    if (todo != null) await SendMessage(todo, update, typeOfTodoList, dto, ct);
                }
            }
        }
        private async Task SendMessage(Todo todo, Update update, TypeOfTodoList typeOfTodoList, CallBackDto dto, CancellationToken ct)
        {
            if (update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

            string text = $"{todo.Name}\n---------\n{todo.Description}\n---------\nдо {todo.FinishedAt}";

            // отправляем сообщение
            await _botClient.EditMessageText(ChatId, MessageId, text, cancellationToken:ct,
                replyMarkup: GetKeybord(typeOfTodoList, dto));
        }

        private InlineKeyboardMarkup GetKeybord(TypeOfTodoList typeOfTodoList, CallBackDto dto)
        {
            var buttons = new List<List<InlineKeyboardButton>>();

            //в зависимости от typeOfTodoList делаем Dto_Action для кнопки НАЗАД
            string dtoAction = typeOfTodoList switch
            {
                TypeOfTodoList.todoAll => Dto_Action.Show,
                TypeOfTodoList.todoOverdue => Dto_Action.ShowOverdue,
                TypeOfTodoList.todoToday => Dto_Action.ShowToday,
                _ => throw new Exception("Нет типа для typeOfTodoList в методе TodoDetailView.SendMessage")
            };

            buttons.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("⬅️ назад", new CallBackDto(Dto_Objects.TodoEditView, dtoAction).ToString()),
                InlineKeyboardButton.WithCallbackData("✅ завершить ", new CallBackDto(
                    Dto_Objects.TodoDoneView, dtoAction, _id: dto.Id).ToString())
            });

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
