using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Components;
using TelegramBot.Components.News;

namespace TelegramNewsBot.Components
{
    public class NewsComponent : IComponent
    {
        ComponentState _State = ComponentState.NotWaitingForInput;

        public string ComponentName { get; } = "News";
        public ComponentState State { get { return _State; } }
        string BaseField { get; set; }
        string CurrentField { get; set; }
        Dictionary<string, string> ProcessedData = new Dictionary<string, string>();
        Dictionary<string, string> Hints;
        Dictionary<string, Handler> DataHandlers;
        Dictionary<string, ServiceHandler> ServiceHandlers;
        ConfigurationModel Config;

        public event StateChangeEventHandler OnStateChange;

        public List<InlineKeyboardMarkup> SourcesKeyboards;
        public List<InlineKeyboardMarkup> CountriesKeyboards;

        public InlineKeyboardMarkup CategoriesKeyboard;
        public InlineKeyboardMarkup CountKeyboard;

        delegate InlineKeyboardMarkup Handler(List<string> callbackData);
        delegate Response ServiceHandler(List<string> callbackData);
        Handler CurrentHandler;

        public NewsComponent()
        {
            Config = Utils.LoadConfig<ConfigurationModel>(@"../../Components/News/Configuration.json");
            Hints = Utils.LoadConfig<Dictionary<string, string>>(@"../../Resources/News/Hints.json");
            var sources = Utils.LoadConfig<Dictionary<string, string>>(@"../../Resources/News/Sources.json");
            var countries = Utils.LoadConfig<Dictionary<string, string>>(@"../../Resources/News/Countries.json");
            var categories = Utils.LoadConfig<Dictionary<string, string>>(@"../../Resources/News/Categories.json");
            var counts = Utils.LoadConfig<Dictionary<string, string>>(@"../../Resources/News/Counts.json");

            SourcesKeyboards = KeyboardManager.InitKeyboard(sources, 3, 3);
            CountriesKeyboards = KeyboardManager.InitKeyboard(countries, 3, 3);
            CategoriesKeyboard = KeyboardManager.InitKeyboard(categories, 1, categories.Count, true)[0];
            CountKeyboard = KeyboardManager.InitKeyboard(counts, 1, counts.Count, true)[0];
            BindHandlers();
        }

        void BindHandlers()
        {
            DataHandlers = new Dictionary<string, Handler>
            {
                { NewsFieldName.Country, new Handler(CountryKeyboard) },
                { NewsFieldName.Category, new Handler(CategoryKeyboard) },
                { NewsFieldName.Q, new Handler(QKeyboard) },
                { NewsFieldName.Count, new Handler(CountsKeyboard) },
                { NewsFieldName.Sources, new Handler(SourcesKeyboard) }
            };
            ServiceHandlers = new Dictionary<string, ServiceHandler>
            {
                { StringTokens.StartToken, new ServiceHandler(Start) },
                { StringTokens.FinishToken, new ServiceHandler(Finish) },
                { StringTokens.SkipToken, new ServiceHandler(Skip) }
            };
        }

        public Response Handle(string callbackData)
        {
            Response response = null;
            List<string> data = callbackData.Split(':').Select(x => x.Trim()).ToList();
            if (callbackData.Contains(StringTokens.BasefieldToken))
            {
                if (BaseField == null)
                {
                    string value = data[1];
                    if (Config.RequestFields.Contains(value) && Config.BaseFields.Contains(value))
                    {
                        BaseField = value;
                        response = ChooseNextHandler(data);
                    }
                }
            }
            if (callbackData.Contains(StringTokens.PageToken))
            {
                var keyboard = CurrentHandler(data);
                var advice = Hints[CurrentField];
                response = new Response(advice, keyboard);
            }
            if (ServiceHandlers.Keys.Contains(callbackData))
            {
                 response = ServiceHandlers[callbackData](data);
            }
            if (callbackData.Contains(StringTokens.ValueToken) || response == null)
            {
                if (_State == ComponentState.WaitingForInput)
                {
                    _State = ComponentState.NotWaitingForInput;
                    var args = new StateChangeEventArgs(_State);
                    OnStateChange(this, args);
                }
                ProcessedData[CurrentField] = data.Last();
                response = ChooseNextHandler(data);
            }
            return response;
        }

        Response ChooseNextHandler(List<string> callbackData)
        {
            InlineKeyboardMarkup keyboard = null;
            string advice = null;
            foreach (string fieldName in Config.RequestFields)
            {
                if (!ProcessedData.ContainsKey(fieldName))
                {
                    if (!Config.IncompatibleFields[BaseField].Contains(fieldName))
                    {
                        CurrentField = fieldName;
                        CurrentHandler = DataHandlers[fieldName];
                        keyboard = CurrentHandler(callbackData);
                        advice = Hints[fieldName];
                        break;
                    }
                }
            }
            if (keyboard == null && advice == null)
            {
                keyboard = FinishKeyboard(callbackData);
                advice = Hints[StringTokens.FinishToken];
            }
            return new Response(advice, keyboard);
        }

        public Response Start(List<string> callbackData)
        {
            var baseFields = Config.BaseFields.ToDictionary(s => s, s => $"{StringTokens.BasefieldToken}{s}");
            InlineKeyboardMarkup keyboard = KeyboardManager.InitKeyboard(baseFields, 1, baseFields.Count)[0];
            var response = new Response(Hints[StringTokens.StartToken], keyboard);
            return response;
        }

        public Response Finish(List<string> callbackData)
        {
            Response response = new Response("Here will be news", null);
            return response;
        }

        public Response Skip(List<string> callbackData)
        {
            Response response;
            if (Config.SkippableFields.Contains(CurrentField))
            {
                ProcessedData[CurrentField] = null;
                if (_State == ComponentState.WaitingForInput)
                {
                    _State = ComponentState.NotWaitingForInput;
                    var args = new StateChangeEventArgs(_State);
                    OnStateChange(this, args);
                }
                response = ChooseNextHandler(callbackData);
            } else
            {
                var keyboard = DataHandlers[CurrentField](callbackData);
                var hint = Hints[CurrentField];
                response = new Response(hint, keyboard);
            }
            return response;
        }

        public InlineKeyboardMarkup FinishKeyboard(List<string> callbackData)
        {
            CurrentField = "";
            CurrentHandler = null;
            BaseField = "";
            var response = new InlineKeyboardMarkup(
                new InlineKeyboardButton {
                    Text = "Finish", CallbackData = StringTokens.FinishToken
                });
            return response;
        }

        public InlineKeyboardMarkup CountryKeyboard(List<string> callbackData)
        {
            int page = ExtractPage(callbackData);
            var response = CountriesKeyboards[page];
            return response;
        }

        public InlineKeyboardMarkup CategoryKeyboard(List<string> callbackData)
        {
            return CategoriesKeyboard;
        }

        public InlineKeyboardMarkup SourcesKeyboard(List<string> callbackData)
        {
            int page = ExtractPage(callbackData);
            var keyboard = SourcesKeyboards[page];
            return keyboard;
        }

        int ExtractPage(List<string> callbackData)
        {
            int page = 0;
            int.TryParse(callbackData.Last(), out page);
            return page;
        }

        public InlineKeyboardMarkup QKeyboard(List<string> callbackData)
        {
            _State = ComponentState.WaitingForInput;
            var args = new StateChangeEventArgs(_State);
            OnStateChange(this, args);
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
                new InlineKeyboardButton { Text = "Skip", CallbackData = StringTokens.SkipToken } 
            );
            return keyboard;
        }

        public InlineKeyboardMarkup CountsKeyboard(List<string> callbackData)
        {
            return CountKeyboard;
        }
    }
}
