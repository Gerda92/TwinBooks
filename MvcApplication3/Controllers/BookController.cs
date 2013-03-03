using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyReading.Models;
using System.IO;
using EasyReading.Lib;

namespace EasyReading.Controllers
{
    public class BookController : Controller
    {
        private BookDbContext db = new BookDbContext();

        //
        // GET: /Book/

        public ActionResult Index()
        {
            return View(db.Books.ToList());
        }

        //
        // GET: /Book/Details/5

        public ActionResult Details(int id = 0)
        {

            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        //
        // GET: /Book/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Book/Create

        [HttpPost]
        public ActionResult Create(Book temp)
        {
            if (Request.Files.Count == 2)
            {
                BookGroup group = new BookGroup();
                for (int i = 0; i < 2; i++)
                {
                    var uploadedFile = Request.Files[i];
                    var fileSavePath = "~/App_Data/UploadedBooks/" + uploadedFile.FileName;
                    uploadedFile.SaveAs(Server.MapPath(fileSavePath));

                    Book b = BookFormatter.ExtractBook(fileSavePath);
                    if (group.Title == null)
                    {
                        group.Title = b.Title;
                        group.Author = b.Author;
                    }

                    group.Books.Add(b);

                }
                db.BookGroups.Add(group);
                db.SaveChanges();

                foreach (Book book in group.Books.ToList()) {
                    BookFormatter.PrepareForAlignment(book);
                }

                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //
        // GET: /Book/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        //
        // POST: /Book/Edit/5

        [HttpPost]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        //
        // GET: /Book/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        //
        // POST: /Book/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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