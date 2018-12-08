using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Components;

namespace TelegramNewsBot.Components
{
    public static class KeyboardManager
    {
        public static List<InlineKeyboardButton> MakeButtons(Dictionary<string, string> data)
        {
            var buttons = new List<InlineKeyboardButton>();
            foreach (var buttonData in data)
            {
                var button = new InlineKeyboardButton
                {
                    Text = buttonData.Key,
                    CallbackData = buttonData.Value
                };
                buttons.Add(button);
            }
            return buttons;
        }

        public static List<List<InlineKeyboardButton>> MakeRows(
            List<InlineKeyboardButton> buttons, int buttonsInRow)
        {
            var rows = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < buttons.Count; i += buttonsInRow)
            {
                var row = new List<InlineKeyboardButton>();
                row.AddRange(buttons.Skip(i).Take(buttonsInRow));
                rows.Add(row);
            }
            return rows;
        }

        public static List<InlineKeyboardMarkup> Paginate(
            List<List<InlineKeyboardButton>> rows, int rowsPerPage, bool skip, bool fewValues)
        {
            var keyboards = new List<InlineKeyboardMarkup>();
            int pagesCount = (int)Math.Ceiling((double)rows.Count / rowsPerPage);
            var skipRow = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = "Skip",
                    CallbackData = StringTokens.SkipToken
                }
            };
            for (int rowsCount = 0, pageNumber = 0;
                rowsCount < rows.Count; rowsCount += rowsPerPage, pageNumber++)
            {
                var pagingRow = new List<InlineKeyboardButton>();
                int nextPage = pageNumber + 1;
                int previousPage = pageNumber - 1; 
                if (previousPage >= 0)
                {
                    var previousButton = new InlineKeyboardButton
                    {
                        Text = "Previous",
                        CallbackData = $"{StringTokens.PageToken}{previousPage}"
                    };
                    pagingRow.Add(previousButton);
                }
                if (nextPage < pagesCount)
                {
                    var nextButton = new InlineKeyboardButton
                    {
                        Text = "Next",
                        CallbackData = $"{StringTokens.PageToken}{nextPage}"
                    };
                    pagingRow.Add(nextButton);
                }
                var keyboardRows = rows.Skip(rowsCount).Take(rowsPerPage).ToList();
                keyboardRows.Add(pagingRow);
                var keyboard = new InlineKeyboardMarkup(keyboardRows);
                if (skip)
                {
                    keyboardRows.Add(skipRow);
                }
                keyboards.Add(keyboard);
            }
            return keyboards;
        }

        public static List<InlineKeyboardMarkup> InitKeyboard(
            Dictionary<string, string> data, int buttonsInRow, int rowsPerPage,
            bool skip = false, bool fewValues = false)
        {
            var buttons = MakeButtons(data);
            var rows = MakeRows(buttons, buttonsInRow);
            var pages = Paginate(rows, rowsPerPage, skip, fewValues);
            return pages;
        }
    }
}
