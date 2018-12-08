using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Components.News
{
    class ConfigurationModel
    {
        public List<string> ResponseFields;
        public List<string> RequestFields;
        public Dictionary<string, List<string>> IncompatibleFields;
        public List<string> BaseFields;
        public List<string> SkippableFields;
        public List<string> FewValuesFields;
        public string UrlTemplate;
    }
}
