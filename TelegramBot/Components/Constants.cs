using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Components
{
    public class StringTokens
    {
        public static readonly string StartToken = "start";
        public static readonly string FinishToken = "finish";
        public static readonly string ValueToken = "value:";
        public static readonly string BasefieldToken = "baseField:";
        public static readonly string PageToken = "page:";
        public static readonly string InputToken = "input";
        public static readonly string ComponentToken = "component:";
        public static readonly string SkipToken = "skip";
        public static readonly string NextToken = "next";
        public static readonly string WrongChoiceHint = "wrongChoice";
    }

    public class NewsFieldName
    {
        public static readonly string Q = "q";
        public static readonly string Country = "country";
        public static readonly string Sources = "sources";
        public static readonly string Category = "category";
        public static readonly string Count = "count";
    }

    public class StringCaptions
    {
        public static readonly string NextPage = "Next page";
        public static readonly string PreviousPage = "Previous page";
        public static readonly string Skip = "Skip";
        public static readonly string Continue = "Continue";
        public static readonly string Finish = "Finish";
    }

    public enum ComponentState
    {
        NotWaitingForInput,
        WaitingForInput,
    }
}
