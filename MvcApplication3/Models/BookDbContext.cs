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
        public DbSet<BookGroup> BookGroups { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChapterBinding> ChapterBindings { get; set; }
        public DbSet<BookmarkBinding> BookmarkBindings { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        public List<ChapterBinding> getChapterBindings(Book first, Book second)
        {
            var binding1 = ChapterBindings.Where(r => (r.ChapterOne.InBook.Id == first.Id
                && r.ChapterTwo.InBook.Id == second.Id)).ToList().Select(ch => new ChapterBinding
                {
                    Id = ch.Id,
                    ChapterOne = ch.ChapterOne,
                    ChapterTwo = ch.ChapterTwo
                });
            var binding2 = ChapterBindings.Where(r => (r.ChapterOne.InBook.Id == second.Id
                && r.ChapterTwo.InBook.Id == first.Id)).ToList().Select(ch => new ChapterBinding
                {
                    Id = ch.Id,
                    ChapterOne = ch.ChapterTwo,
                    ChapterTwo = ch.ChapterOne
                });

            var binding = binding1;
            if (binding == null) binding = binding2;
            else if (binding2 != null) binding = binding.Union(binding2);

            return binding.ToList();
        }


        
        public IEnumerable<BookmarkBinding> getBookmarks(int book1, int book2)
        {
            var bm1 = Bookmarks.Where(r => r.InBook.Id == book1).OrderBy(r => r.Order);
            var bm2 = Bookmarks.Where(r => r.InBook.Id == book2).OrderBy(r => r.Order);
            var bindings = getBookmarkBindings(book1, book2).OrderByDescending(r => r.Type)
                .ThenByDescending(r => r.CreatedAt).ToList();
            for (int i = 0; i < bindings.Count; i++ )
            {
                var b = bindings.ElementAt(i);
                var affected = bindings.Where(r => ((r.Bookmark1.Order - b.Bookmark1.Order) * (r.Bookmark2.Order - b.Bookmark2.Order) <= 0)
                    && (r.CreatedAt < b.CreatedAt || r.Type < b.Type));
                bindings.RemoveAll(r => ((r.Bookmark1.Order - b.Bookmark1.Order) * (r.Bookmark2.Order - b.Bookmark2.Order) <= 0)
                    && (r.CreatedAt < b.CreatedAt || r.Type < b.Type));
            }
            return bindings.OrderBy(r => r.Bookmark1.Order);
        }
        
        public IQueryable<BookmarkBinding> getBookmarkBindings(int book1, int book2)
        {
            var binding = BookmarkBindings.Where(r => (r.Bookmark1.InBook.Id == book1
                && r.Bookmark2.InBook.Id == book2) || (r.Bookmark1.InBook.Id == book2
                && r.Bookmark2.InBook.Id == book1));

            return binding.OrderBy(r => r.Bookmark1.Order);
        }
    

    }
}