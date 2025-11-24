using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
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
    public class TodoDoneView : BaseView
    {
        private readonly ITodoService _todoService;
        private readonly TodoMenuView _todoMenuView;

        public TodoDoneView(ITelegramBotClient botClient, ITodoService todoService,
            TodoMenuView todoMenuView) : base(botClient)
        {
            _todoService = todoService;
            _todoMenuView = todoMenuView;
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);

            //получить dto
            if(Text != null)
            {
                var dto = CallBackDto.FromString(Text);

                //если dto.Action == Dto_Action_TodoDone то делаем задачу завершенной
                if(dto.Action == Dto_Action.TodoDone)
                {
                    await TodoDoneAsync(update, dto, ct);
                    return;
                }

                //в зависимости от dto.Action получаем typeOfTodoList
                TypeOfTodoList typeOfTodoList = dto.Action switch
                {
                    nameof(Dto_Action.Show) => TypeOfTodoList.todoAll,
                    nameof(Dto_Action.ShowOverdue) => TypeOfTodoList.todoOverdue,
                    nameof(Dto_Action.ShowToday) => TypeOfTodoList.todoToday,
                    _ => throw new Exception("нет типа в методе TodoDoneView.Show")
                };

                //получить задачу
                if (dto.Id != null)
                {
                    var todo = await _todoService.GetTodoById((Guid)dto.Id, ct);

                    //отправляем сообщение
                    await SendMessage(update, typeOfTodoList, ct);
                }
            }
        }

        private async Task SendMessage(Update update, TypeOfTodoList typeOfTodoList, CancellationToken ct)
        {
            if(Text != null)
            {
                var dto = CallBackDto.FromString(Text);

                //получаем задачу
                if(dto.Id != null)
                {
                    var todo = await _todoService.GetTodoById((Guid)dto.Id, ct);

                    //в зависимости от typeOfTodoList делаем dto для кнопки НЕТ
                    var dtoBackButton = typeOfTodoList switch
                    {
                        TypeOfTodoList.todoAll => Dto_Action.Show,
                        TypeOfTodoList.todoOverdue => Dto_Action.ShowOverdue,
                        TypeOfTodoList.todoToday => Dto_Action.ShowToday,
                        _ => throw new Exception("нет типа в методе TodoDoneView.SendMessage")
                    };

                    if (todo != null)
                    {
                        //отправляем сообщение
                        if (update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

                        var text = $"Вы действительно хотите сделать задачу {todo.Name} завершенной?";
                        var buttons = new List<InlineKeyboardButton>() 
                            { 
                                InlineKeyboardButton.WithCallbackData("⬅️ назад",new CallBackDto
                                    (Dto_Objects.TodoDetailView, dtoBackButton, _id: dto.Id).ToString()),
                                InlineKeyboardButton.WithCallbackData("✅ да",new CallBackDto
                                    (Dto_Objects.TodoDoneView,Dto_Action.TodoDone, _id: dto.Id).ToString())
                            };

                        await _botClient.EditMessageText(ChatId, MessageId, text,
                            cancellationToken:ct,
                            replyMarkup: new InlineKeyboardMarkup(buttons));
                    }
                }
            }
        }

        private async Task TodoDoneAsync(Update update, CallBackDto dto, CancellationToken ct)
        {
            if(dto.Id != null)
            {
                //получаем задачу
                var todo = await _todoService.GetTodoById((Guid)dto.Id, ct);

                if(todo != null)
                {
                    //делаем задачу завершенной
                    var isCompleted = await _todoService.MarkAsCompletedAsync(todo.Id, ct);

                    //переходим в меню Мои задачи
                    await _todoMenuView.Show(update, ct);
                }
            }
        }
    }
}
