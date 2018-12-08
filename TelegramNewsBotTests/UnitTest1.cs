using System;
using TelegramNewsBot.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;

namespace TelegramNewsBotTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestJsonLoading()
        {
            string rawJsonPath = @"../../Resources/test.json";
            string listJsonPath = @"../../Resources/test1.json";
            var rawJson = Utils.LoadConfig
                <Dictionary<string, string>>(rawJsonPath);
            var arrayJson = Utils.LoadConfig
                <Dictionary<string, List<string>>>(listJsonPath);
            string s = rawJson["foo"];
            string s1 = arrayJson["foo"][0];
            Assert.AreEqual(s, "bar");
            Assert.AreEqual(s1, "bar");
        }

        [TestMethod]
        public void TestKeyboardMaking()
        {
            var data = new Dictionary<string, string>();
            for (int i = 0; i < 5; i++)
                data.Add($"text{i}", $"data{i}");
            var buttons = KeyboardManager.MakeButtons(data);

            var rows1 = KeyboardManager.MakeRows(buttons, 1);
            var rows2 = KeyboardManager.MakeRows(buttons, 2);
            var rows3 = KeyboardManager.MakeRows(buttons, 6);

            var keyboards1 = KeyboardManager.Paginate(rows1, 1);

            Assert.AreEqual(buttons[2].CallbackData, "data2");
            Assert.AreEqual(rows1[1][0].CallbackData, "data1");
            Assert.AreEqual(rows2[0][1].CallbackData, "data1");
            Assert.AreEqual(rows3[0][3].CallbackData, "data3");
            
            var pageFirst = keyboards1[0];
            var lastPage = keyboards1[keyboards1.Count - 1];
            var middlePage = keyboards1[2];

            var pages = pageFirst.InlineKeyboard.ToList();
            var firstNextButton = pages[1].ToList()[0];

            pages = lastPage.InlineKeyboard.ToList();
            var lastPrevButton = pages[1].ToList()[0];

            pages = middlePage.InlineKeyboard.ToList();
            var nextButton = pages[1].ToList()[1];
            var prevButton = pages[1].ToList()[0];

            Console.WriteLine(firstNextButton.CallbackData);
            Console.WriteLine(lastPrevButton.CallbackData);
            Assert.AreEqual(firstNextButton.CallbackData, "Page: 1");
            Assert.AreEqual(lastPrevButton.CallbackData, $"Page: {keyboards1.Count - 2}");
            Assert.AreEqual(nextButton.CallbackData, "Page: 3");
            Assert.AreEqual(prevButton.CallbackData, "Page: 1");
        }
    }
}
