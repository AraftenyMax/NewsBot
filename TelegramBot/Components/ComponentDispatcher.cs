using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Components;

namespace TelegramNewsBot.Components
{

    public class ComponentDispatcher
    {
        List<IComponent> Components = new List<IComponent>() {
            new NewsComponent()
        };
        ComponentState _State = ComponentState.NotWaitingForInput;
        public ComponentState State { get { return _State; } }
        string StartAdvice = "Choose what do you want to know";
        Dictionary<string, IComponent> ComponentsList = new Dictionary<string, IComponent>();
        Dictionary<string, string> ComponentNames = new Dictionary<string, string>();
        InlineKeyboardMarkup ComponentsKeyboard;
        IComponent CurrentComponent { get; set; }

        public ComponentDispatcher()
        {
            Components.ForEach(x => ComponentsList[x.ComponentName] = x);
            Components.ForEach(x => ComponentNames[x.ComponentName] =
            $"{StringTokens.ComponentToken}{x.ComponentName}");
            ComponentsKeyboard = KeyboardManager.InitKeyboard(ComponentNames, 1, ComponentsList.Count)[0];
        }

        void OnStateChangeHandler(object sender, StateChangeEventArgs args)
        {
            _State = args.State;
        }

        void OnStateChange(ComponentState newState)
        {
            _State = newState;
        }

        public bool IsStart(string callbackData)
        {
            return callbackData.Contains(StringTokens.FinishToken);
        }

        public bool IsComponent(string callbackData)
        {
            return callbackData.Contains(StringTokens.ComponentToken);
        }

        public bool IsFinish(string callbackData)
        {
            return callbackData.Equals(StringTokens.FinishToken);
        }

        public Response Start()
        {
            return new Response(StartAdvice, ComponentsKeyboard);
        }

        public Response Handle(string callbackData)
        {
            Response response = null;
            if (callbackData.Contains(StringTokens.StartToken))
            {
                return Start();
            }
            if (IsComponent(callbackData))
            {
                string componentName = callbackData.Split(':')[1].Trim();
                IComponent component = Components.FirstOrDefault(x => x.ComponentName == componentName);
                if (CurrentComponent == null)
                {
                    response = component.Handle(StringTokens.StartToken);
                    CurrentComponent = component;
                    CurrentComponent.OnStateChange += OnStateChangeHandler;
                }
            }
            if (State == ComponentState.WaitingForInput)
            {
                callbackData = StringTokens.ValueToken + callbackData;
            }
            if (response == null)
            {
                response = CurrentComponent.Handle(callbackData);
            }
            if (IsFinish(callbackData))
            {
                CurrentComponent.OnStateChange -= OnStateChangeHandler;
                CurrentComponent = null;
            }
            return response;
        }
    }
}
