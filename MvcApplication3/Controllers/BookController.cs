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

                List<Book> books = new List<Book>();
                for (int i = 0; i < 2; i++)
                {
                    var uploadedFile = Request.Files[i];
                    var fileSavePath = "~/App_Data/UploadedBooks/" + uploadedFile.FileName;
                    uploadedFile.SaveAs(Server.MapPath(fileSavePath));

                    Book b = BookFormatter.ExtractBook(fileSavePath);

                    books.Add(b);

                    db.Books.Add(b);

                }

                db.SaveChanges();

                foreach (Book book in books) {
                    BookFormatter.PrepareForAlignment(book);
                }

                db.SaveChanges();

                return RedirectToAction("AlignChapters", "Alignment", new { id1 = books[0].Id, id2 = books[1].Id });
            }
            return View();
        }

    }
}