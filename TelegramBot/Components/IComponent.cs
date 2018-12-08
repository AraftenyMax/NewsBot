using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Components;

namespace TelegramBot.Components
{
    interface IComponent
    {
        string ComponentName { get; }
        event StateChangeEventHandler OnStateChange;
        Response Handle(string callbackData);
    }
}
