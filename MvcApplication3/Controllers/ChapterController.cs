using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyReading.Models;

namespace EasyReading.Controllers
{
    public class ChapterController : Controller
    {
        private BookDbContext db = new BookDbContext();

        //
        // GET: /Chapter/

        [HttpGet]
        public ActionResult Index(int book_id)
        {
            var chapters = db.Books.Single(b => b.Id == book_id).Chapters;
            /*
            var collection = chapters.Select(x => new {
                Id = x.ChapterId,
                Order = x.Order
            });
             * */
            //return Json(collection, JsonRequestBehavior.AllowGet);
            return View(chapters);
        }

        public ActionResult Index()
        {
            return View(db.Chapters.ToList());
        }

        //
        // GET: /Chapter/Details/5

        public ActionResult Details(string id = null)
        {
            Chapter chapter = db.Chapters.Find(id);
            var p = chapter.Parent;
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        //
        // GET: /Chapter/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Chapter/Create

        [HttpPost]
        public ActionResult Create(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chapter);
        }

        //
        // GET: /Chapter/Edit/5

        public ActionResult Edit(string id = null)
        {
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        //
        // POST: /Chapter/Edit/5

        [HttpPost]
        public ActionResult Edit(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chapter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chapter);
        }

        //
        // GET: /Chapter/Delete/5

        public ActionResult Delete(string id = null)
        {
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        //
        // POST: /Chapter/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            Chapter chapter = db.Chapters.Find(id);
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}