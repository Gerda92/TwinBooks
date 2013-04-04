using EasyReading.Lib;
using EasyReading.Models;
using MvcApplication3.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            TwinBook tb = new TwinBook()
                {
                    Book1 = book1,
                    Book2 = book2
                };
            try
            {
                tb = db.TwinBooks.Single(t => t.Book1.Id == id1 && t.Book2.Id == id2);
            }
            catch (Exception e)
            {
                tb = new TwinBook()
                {
                    Book1 = book1,
                    Book2 = book2
                };
                db.TwinBooks.Add(tb);
                db.SaveChanges();
            }

            return View(tb);
        }

        //
        // GET: Alignment/CreateChapter/{id1}/{id2}

        [HttpGet]
        public JsonResult CreateChapterBinding(string id1, string id2 = null)
        {

            var ch1 = db.Chapters.Single(r => r.ChapterId == id1);
            var ch2 = id2 == null ? null : db.Chapters.Single(r => r.ChapterId == id2);

            var tb = db.TwinBooks.Single(t => t.Book1.Id == ch1.InBook.Id && t.Book2.Id == ch2.InBook.Id);

            ChapterBinding cb = new ChapterBinding()
            {
                ChapterOne = ch1,
                ChapterTwo = ch2
            };

            tb.Chapters.Add(cb);
            db.SaveChanges();

            return Json("Success!", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateTwinBook(int id)
        {
            var tb = db.TwinBooks.Find(id);

            var twinPath = BookFormatter.CreateTweenBook(tb);

            tb.Path = twinPath;

            var bms = BookAligner.AlignByChapters(tb);

            tb.Bookmarks = bms;

            db.SaveChanges();

            return RedirectToAction("GetTwinBook", new {id = tb.Id});
        }

        [HttpGet]
        public ActionResult GetTwinBook(int id)
        {
            TwinBook tb = db.TwinBooks.Find(id);

            //db.reactivate(tb);

            //db.SaveChanges();

            var marks = tb.Bookmarks.Where(b => b.Active == true).OrderBy(r => r.Bookmark1.Order).Select(
                b => new
                {
                    Id = b.Id,
                    BookId1 = b.Bookmark1.InBook.Id,
                    BookId2 = b.Bookmark2.InBook.Id,
                    BookmarkId1 = b.Bookmark1.BookmarkId,
                    BookmarkId2 = b.Bookmark2.BookmarkId,
                    Order1 = b.Bookmark1.Order,
                    Order2 = b.Bookmark2.Order,
                    Type = b.Type
                }
            );

            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(marks);

            ViewBag.Alignments = sJSON;

            return View(tb);
        }

        //
        // GET: Alignment/CreateBookmark/{id1}/{id2}

        [HttpGet]
        public JsonResult CreateBookmarkBinding(int twinId, string id1, string id2 = null)
        {

            var tb = db.TwinBooks.Find(twinId);

            Bookmark bm1 = db.Bookmarks.Single(r => (r.InBook.Id == tb.Book1.Id) && (r.BookmarkId == id1));
            Bookmark bm2 = db.Bookmarks.Single(r => (r.InBook.Id == tb.Book2.Id) && (r.BookmarkId == id2));

            // creating new bookmark

            BookmarkBinding mark = new BookmarkBinding()
            {
                Bookmark1 = bm1,
                Bookmark2 = bm2,
                Type = 1,
                Active = true
            };

            // get active bookmarks

            var active = tb.Bookmarks.Where(b => b.Active == true && b.Type == 1).OrderBy(r => r.Bookmark1.Order);

            // get touched region

            var affected = active.Where(r => ((r.Bookmark1.Order - mark.Bookmark1.Order) * (r.Bookmark2.Order - mark.Bookmark2.Order) <= 0)
                    && (r.CreatedAt < mark.CreatedAt));

            //set inactive or delete

            foreach (var b in affected)
            {
                b.Active = false;
            }

            var upper = active.LastOrDefault(b => b.Bookmark1.Order < mark.Bookmark1.Order && b.Active == true && b.Type == 1);
            if (upper == null) upper = tb.Bookmarks.OrderBy(r => r.Bookmark1.Order).First();
            var lower = active.FirstOrDefault(b => b.Bookmark1.Order > mark.Bookmark1.Order && b.Active == true && b.Type == 1);
            if (lower == null) lower = tb.Bookmarks.OrderBy(r => r.Bookmark1.Order).Last();

            var realign = BookAligner.AlignInRange(upper, mark);
            realign.Add(mark);
            realign.AddRange(BookAligner.AlignInRange(mark, lower));

            var toDelete = tb.Bookmarks.Where(r => (r.Bookmark1.Order >= upper.Bookmark1.Order
                && r.Bookmark1.Order <= lower.Bookmark1.Order) && (r.Type == 0)).ToList();

            var inactive = tb.Bookmarks.Where(r => (r.Active == false) && (r.Type == 0)).ToList();
            toDelete.AddRange(inactive);

            foreach (var b in toDelete)
            {
                tb.Bookmarks.Remove(b);
            }

            foreach (var b in realign) {
                tb.Bookmarks.Add(b);
            }
            

            db.SaveChanges();

            // get realigned fragment

            var marks = tb.Bookmarks.Where(b => b.Active == true).OrderBy(b => b.Bookmark1.Order).Select(
                    b => new
                    {
                        Id = b.Id,
                        BookId1 = b.Bookmark1.InBook.Id,
                        BookId2 = b.Bookmark2.InBook.Id,
                        BookmarkId1 = b.Bookmark1.BookmarkId,
                        BookmarkId2 = b.Bookmark2.BookmarkId,
                        Order1 = b.Bookmark1.Order,
                        Order2 = b.Bookmark2.Order,
                        Type = b.Type
                    }
                );


            return Json(marks, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookmarkBinding(int twinId)
        {

            var tb = db.TwinBooks.Find(twinId);

            //db.reactivate(tb);

            //db.SaveChanges();

            // get realigned fragment

            var marks = tb.Bookmarks.Where(b => b.Active == true).OrderBy(b => b.Bookmark1.Order).Select(
                    b => new
                    {
                        Id = b.Id,
                        BookId1 = b.Bookmark1.InBook.Id,
                        BookId2 = b.Bookmark2.InBook.Id,
                        BookmarkId1 = b.Bookmark1.BookmarkId,
                        BookmarkId2 = b.Bookmark2.BookmarkId,
                        Order1 = b.Bookmark1.Order,
                        Order2 = b.Bookmark2.Order,
                        Type = b.Type
                    }
                );


            return Json(marks, JsonRequestBehavior.AllowGet);
        }

    }
}
