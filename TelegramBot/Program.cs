using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot;

namespace TelegramNewsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot client = new Bot();
            client.Start();
        }
    }
}
