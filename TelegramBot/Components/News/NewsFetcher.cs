using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Components.News
{
    class NewsFetcher
    {
        const int DefaultArticlesCount = 5;
        string UrlTemplate;
        List<string> ArgNames;
        public NewsFetcher(string urlTemplate, List<string> argNames)
        {
            UrlTemplate = urlTemplate;
            ArgNames = argNames;
        }

        public string BuildUrl(Dictionary<string, string> args)
        {
            string urlArgs = null;
            foreach (var fieldData in args)
            {
                if (ArgNames.Contains(fieldData.Key))
                {
                    if (urlArgs == null)
                    {
                        urlArgs = $"{fieldData.Key}={fieldData.Value}";
                    }
                    else
                    {
                        urlArgs += $"&{fieldData.Key}={fieldData.Value}";
                    }
                }
            }
            string url = String.Format(UrlTemplate, urlArgs);
            return url;
        }

        public ArticleModel RequestNews(string url)
        {
            string json;
            using(var webClient = new WebClient())
            {
                json = webClient.DownloadString(url);
            }
            var data = JsonConvert.DeserializeObject<ArticleModel>(json);
            return data;
        }

        public string FormatResponse(ArticleModel data, int articlesCount = DefaultArticlesCount)
        {
            int articlesParsed = 0;
            var articles = new List<string>();
            foreach(var article in data.articles)
            {
                if (articlesParsed == articlesCount)
                    break;
                articles.Add(article.ToString());
                articlesParsed++;
            }
            return String.Join("\n", articles);
        }

        public string FetchNews(Dictionary<string, string> args)
        {
            string url = BuildUrl(args);
            var data = RequestNews(url);
            int articlesCount = args.ContainsKey(NewsFieldName.Count) ?
                int.Parse(args[NewsFieldName.Count]) : DefaultArticlesCount;
            var response = FormatResponse(data, articlesCount);
            return response;
        }
    }
}
