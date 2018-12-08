using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
   
namespace TelegramNewsBot.Components
{
    public class Utils
    {
        public static T LoadConfig<T>(string source_name)
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(source_name));
            return data;
        }

        public static string Captialize(string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
