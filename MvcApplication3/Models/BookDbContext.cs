using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MoreLinq;

namespace EasyReading.Models
{
    public class BookDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<TwinBook> TwinBooks { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChapterBinding> ChapterBindings { get; set; }
        public DbSet<BookmarkBinding> BookmarkBindings { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
  
        public void reactivate(TwinBook tb)
        {
            var bindings = tb.Bookmarks
                .OrderByDescending(r => r.CreatedAt).ToList();
            for (int i = 0; i < bindings.Count; i++ )
            {

                var b = bindings.ElementAt(i);
                tb.Bookmarks.Single(r => r.Id == b.Id).Active = true;
                var affected = tb.Bookmarks.Where(r => ((r.Bookmark1.Order - b.Bookmark1.Order) * (r.Bookmark2.Order - b.Bookmark2.Order) <= 0)
                    && ((r.CreatedAt < b.CreatedAt && r.Type == b.Type) || r.Type < b.Type));
                foreach (var a in affected)
                {
                    a.Active = false;
                }
                bindings.RemoveAll(r => ((r.Bookmark1.Order - b.Bookmark1.Order) * (r.Bookmark2.Order - b.Bookmark2.Order) <= 0)
                    && (r.CreatedAt < b.CreatedAt));
            }

        }

    }
}