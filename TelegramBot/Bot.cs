using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Components;
using TelegramNewsBot.Components;

namespace TelegramBot
{
    public class Bot
    {
        static readonly TelegramBotClient Client = new TelegramBotClient(
            "{0}");
        ComponentDispatcher dispatcher = new ComponentDispatcher();
        public void Start()
        {
            var bot = Client.GetMeAsync().Result;
            Client.OnMessage += OnMessageReceived;
            Client.OnCallbackQuery += OnCallbackQuery;
            Client.StartReceiving(Array.Empty<UpdateType>());
            Console.ReadKey();
            Client.StopReceiving();
        }

        private async void OnCallbackQuery(object sender, CallbackQueryEventArgs callbackQuery)
        {
            var callbackData = callbackQuery.CallbackQuery.Data.Replace(@"/", String.Empty);
            var response = dispatcher.Handle(callbackData);
            if (response.Keyboard == null)
            {
                await Client.SendTextMessageAsync(
                   callbackQuery.CallbackQuery.Message.Chat.Id, text: response.Hint);
            } else
            {
                await Client.SendTextMessageAsync(
                   callbackQuery.CallbackQuery.Message.Chat.Id, text: response.Hint, replyMarkup: response.Keyboard);
            }
        }

        private async void OnMessageReceived(object sender, MessageEventArgs msgEventArgs)
        {
            var message = msgEventArgs.Message.Text;
            if (dispatcher.State == ComponentState.WaitingForInput || message.StartsWith("/"))
            {
                var callbackData = msgEventArgs.Message.Text.Replace(@"/", String.Empty);
                var response = dispatcher.Handle(callbackData);
                await Client.SendTextMessageAsync(
                msgEventArgs.Message.Chat.Id, text: response.Hint, replyMarkup: response.Keyboard);
            }
        }
    }
}
