﻿using EasyReading.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.Lib
{
    public class BookAligner
    {
        public static List<BookmarkBinding> AlignByChapters(TwinBook tb)
        {
            Book book1 = tb.Book1;
            Book book2 = tb.Book2;
            string path = tb.Path;
            HtmlDocument doc = new HtmlDocument();
            doc.Load(System.Web.Hosting.HostingEnvironment.MapPath(path));
            var left = doc.DocumentNode.SelectSingleNode("//body//*[@class='left-twin']");
            var right = doc.DocumentNode.SelectSingleNode("//body//*[@class='right-twin']");

            var bindings = new List<BookmarkBinding>();

            int count = 0;
            var chapters2 = SelectChapters(right);
            foreach (HtmlNode chapter1 in SelectChapters(left))
            {
                var raw = AlignChunks(chapter1.InnerHtml, chapters2.ElementAt(count).InnerHtml);

                foreach (var b in raw)
                {
                    Bookmark bm1 = book1.Bookmarks.Single(r => r.BookmarkId == b.Bookmark1.BookmarkId);
                    Bookmark bm2 = book2.Bookmarks.Single(r => r.BookmarkId == b.Bookmark2.BookmarkId);
                    BookmarkBinding bm = new BookmarkBinding()
                    {
                        Bookmark1 = bm1,
                        Bookmark2 = bm2,
                        Type = 0
                    };
                    bindings.Add(bm);

                }
                count++;
            }



            return bindings;
        }
        public static List<BookmarkBinding> AlignInRange(BookmarkBinding bm1, BookmarkBinding bm2)
        {
            var bindings = new List<BookmarkBinding>();

            List<Bookmark> marks1 = bm1.Bookmark1.InBook.Bookmarks
                .Where(b => b.Order > bm1.Bookmark1.Order & b.Order < bm2.Bookmark1.Order).OrderBy(b => b.Order).ToList();

            List<Bookmark> marks2 = bm1.Bookmark2.InBook.Bookmarks
                .Where(b => b.Order > bm1.Bookmark2.Order & b.Order < bm2.Bookmark2.Order).OrderBy(b => b.Order).ToList();

            int count = 0;
            foreach (var b1 in marks1)
            {
                if (count >= marks1.Count || count >= marks2.Count) break;
                var b2 = marks2.ElementAt(count);
                BookmarkBinding bm = new BookmarkBinding()
                {
                    Bookmark1 = b1,
                    Bookmark2 = b2,
                    Type = 0
                };
                bindings.Add(bm);
                count++;
            }

            return bindings;
        }

        private static HtmlNodeCollection getSentencesBetween(HtmlNode root, string id1, string id2)
        {
            var sentences = root.SelectNodes("//*[@id='" + id1 +
                "']/following-sibling::*[following-sibling::*[@id='" + id2 + "'][1]]");
            sentences.RemoveAt(0);
            return sentences;
        }

        private static List<BookmarkBinding> AlignChunks(string chunk1, string chunk2)
        {
            var alignments = new List<BookmarkBinding>();

            HtmlDocument doc1 = new HtmlDocument();
            doc1.LoadHtml(chunk1);
            HtmlDocument doc2 = new HtmlDocument();
            doc2.LoadHtml(chunk2);

            HtmlNodeCollection sentences1 = SelectSentences(doc1.DocumentNode);
            HtmlNodeCollection sentences2 = SelectSentences(doc2.DocumentNode);

            int count = 0;
            foreach (HtmlNode s in sentences1)
            {
                if (sentences2.Count <= count) break;
                var bm = new BookmarkBinding
                {
                    Bookmark1 = new Bookmark { BookmarkId = s.Id },
                    Bookmark2 = new Bookmark { BookmarkId = sentences2.ElementAt(count).Id }
                };
                alignments.Add(bm);
                count++;
            }

            return alignments;
        }
        private static HtmlNodeCollection SelectSentences(HtmlNode root)
        {
            return root.SelectNodes("//*[@class='sentence']");
        }

        private static HtmlNodeCollection SelectChapters(HtmlNode root)
        {
            return root.SelectNodes("*[@class='chapter_div']");
        }
    }
}