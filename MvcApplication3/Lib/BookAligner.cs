using EasyReading.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.Lib
{
    public class BookAligner
    {
        public static List<BookmarkBinding> Align(Book book1, Book book2)
        {
            HtmlDocument doc1 = new HtmlDocument();
            doc1.Load(System.Web.Hosting.HostingEnvironment.MapPath(book1.Path));
            HtmlDocument doc2 = new HtmlDocument();
            doc2.Load(System.Web.Hosting.HostingEnvironment.MapPath(book2.Path));

            HtmlNodeCollection sentences1 = SelectSentences(doc1.DocumentNode);
            HtmlNodeCollection sentences2 = SelectSentences(doc2.DocumentNode);

            List<BookmarkBinding> bindings = new List<BookmarkBinding>();
            int count = 0;
            foreach (HtmlNode s in sentences1) {
                if (sentences2.Count <= count) break;
                Bookmark bm1 = book1.Bookmarks.Single(r => r.BookmarkId == s.Id);
                Bookmark bm2 = book1.Bookmarks.Single(r => r.BookmarkId == sentences2.ElementAt(count).Id);
                BookmarkBinding bm = new BookmarkBinding() {
                    Bookmark1 = bm1,
                    Bookmark2 = bm2
                };
                count++;
            }
            return bindings;
        }
        private static HtmlNodeCollection SelectSentences(HtmlNode root)
        {
            return root.SelectNodes("//*[@class='sentence']");
        }
    }
}-