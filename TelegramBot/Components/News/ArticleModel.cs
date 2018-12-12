using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Components.News
{
    class ArticleModel
    {
        public string status;
        public int totalResults;
        public List<Article> articles;
    }

    class Article
    {
        public string author;
        public string title;
        public string description;
        public string url;
        public string urlToImage;
        public string publishedAt;
        public string content;
        public Source source;

        new public string ToString()
        {
            string str = $"Author: {author ?? "Unknown"}\n" +
                $"Title: {title ?? "Untitled"}\n" +
                $"Description: {description ?? "No description"}\n" +
                $"Url: {url}\n" +
                $"Published at: {publishedAt ?? "Unknown"}" +
                $"Source: {source.name ?? "Unknown"}";
            return str;
        }
    }

    class Source
    {
        public string id;
        public string name;
    }
}
