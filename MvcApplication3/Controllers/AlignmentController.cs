using EasyReading.Lib;
using EasyReading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyReading.Controllers
{
    public class AlignmentController : Controller
    {

        private BookDbContext db = new BookDbContext();

        [HttpGet]
        public ActionResult AlignChapters(int id1, int id2)
        {
            var book1 = db.Books.Find(id1);
            var book2 = db.Books.Find(id2);
            return View(new Object[] {book1, book2});
        }

        //
        // GET: Alignment/CreateChapter/{id1}/{id2}

        [HttpGet]
        public JsonResult CreateChapterBinding(string id1, string id2 = null)
        {
            var ch1 = db.Chapters.Single(r => r.ChapterId == id1);
            var ch2 = id2 == null ? null : db.Chapters.Single(r => r.ChapterId == id2);

            ChapterBinding cb = new ChapterBinding()
            {
                ChapterOne = ch1,
                ChapterTwo = ch2
            };

            db.ChapterBindings.Add(cb);
            db.SaveChanges();

            return Json("Success!", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AlignSentences(int id1, int id2)
        {
            var book1 = db.Books.Find(id1);
            var book2 = db.Books.Find(id2);

            var chapters1 = db.getBindedChapters(book1, book2);
            var chapters2 = db.getBindedChapters(book2, book1);

            var twin = BookFormatter.CreateTweenBook(book1, book2, chapters1, chapters2);
            var html = BookFormatter.GetHtmlBody(twin);


            return View(new Object[] { html });
        }

        //
        // GET: Alignment/CreateBookmark/{id1}/{id2}

        [HttpGet]
        public JsonResult CreateBookmarkBinding(int book1, int book2, string id1, string id2 = null)
        {

            BookmarkBinding mark = new BookmarkBinding()
            {
                BookId1 = book1,
                BookId2 = book2,
                BookmarkId1 = id1,
                BookmarkId2 = id2,
                Order1 = BookFormatter.getOrder(id1),
                Order2 = BookFormatter.getOrder(id2)
            };

            db.BookmarkBindings.Add(mark);
            db.SaveChanges();

            return Json(mark, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookmarkBinding(int book1, int book2)
        {

            var marks = db.getBookmarkBindings(book1, book2);

            return Json(marks, JsonRequestBehavior.AllowGet);
        }

    }
}
