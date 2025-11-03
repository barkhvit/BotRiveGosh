using BotRiveGosh.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Handlers.Keyboards
{
    public static class GetKeybords
    {
        public static InlineKeyboardMarkup MainMenu()
        {
            List<List<InlineKeyboardButton>> buttons = new()
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Kpi кассира",new CallBackDto(Dto_Objects.Kpi, Dto_Action.ShowMenu).ToString())
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("О боте", new CallBackDto(Dto_Objects.AboutBotView, Dto_Action.AboutBotShow).ToString())
                }
            };
            return new InlineKeyboardMarkup(buttons);
        }
    }
}
