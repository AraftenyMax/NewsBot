using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Components
{
    public class Response
    {
        public string Hint;
        public InlineKeyboardMarkup Keyboard;
        public string Data;

        public Response() { }

        public Response(string hint, InlineKeyboardMarkup keyboard, string data = "")
        {
            Hint = hint;
            Keyboard = keyboard;
            Data = data;
        }
    }
}
