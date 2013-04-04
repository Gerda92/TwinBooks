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
using System.Data.Objects.SqlClient;

namespace EasyReading.Controllers
{
    public class BookController : Controller
    {
        private BookDbContext db = new BookDbContext();

        [HttpGet]
        public ActionResult GetBooks()
        {
            var books = db.TwinBooks.Select(
                    b => new
                    {
                        Id = b.Id,
                        Book1 = new { Title = b.Book1.Title, Author = b.Book1.Author, Language = b.Book1.Language },
                        Book2 = new { Title = b.Book2.Title, Author = b.Book2.Author, Language = b.Book2.Language }
                    }
                );            

            return Json(books, JsonRequestBehavior.AllowGet);
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

        public void Download(int id)
        {
            var tb = db.TwinBooks.Find(id);

            string path = Server.MapPath(tb.Path);
            string name = Path.GetFileName(path);
            string ext = Path.GetExtension(path);
            string type = "";

            // set known types based on file extension  
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".htm":
                    case ".html":
                        type = "text/HTML";
                        break;

                    case ".txt":
                        type = "text/plain";
                        break;

                    case ".doc":
                    case ".rtf":
                        type = "Application/msword";
                        break;
                }
            }
            Response.AppendHeader("content-disposition", 
                "attachment; filename=" + name);
            if (type != "")
                Response.ContentType = type;
            Response.WriteFile(path);
            Response.End();
        }

    }
}