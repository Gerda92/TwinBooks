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

        public List<Chapter> getBindedChapters(Book first, Book second)
        {
            var bindings = getChapterBindings(first, second);
            var chapters = bindings.Select(ch => new Chapter
            {
                Id = ch.ChapterOne.Id,
                ChapterId = ch.ChapterOne.ChapterId,
                InBook = ch.ChapterOne.InBook,
                Order = ch.ChapterOne.Order
            }).OrderBy(ch => ch.Order).DistinctBy(ch => ch.Id);
            return chapters.ToList();
        }

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

        public List<BookmarkBinding> getBookmarkBindings(int book1, int book2)
        {
            var binding2 = BookmarkBindings.Where(r => (r.BookId1 == book1
                && r.BookId2 == book2));
            var binding1 = BookmarkBindings.Where(r => (r.BookId1 == book2
                && r.BookId2 == book1));
                /*.ToList().Select(r => new BookmarkBinding
                {
                    Id = r.Id,
                    BookId1 = r.BookId2,
                    BookId2 = r.BookId1,
                    BookmarkId1 = r.BookmarkId2,
                    BookmarkId2 = r.BookmarkId1
                });*/

            var binding = binding2;
            //if (binding == null) binding = binding2;
            //else if (binding2 != null) binding = binding.Union(binding2);

            return binding.OrderBy(r => r.Order1).ToList();
        }

    }
}