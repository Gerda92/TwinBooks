using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyReading.Models
{
    public class TwinBook
    {
        [Key]
        public int Id { get; set; }

        public virtual Book Book1 { get; set; }
        public virtual Book Book2 { get; set; }

        public string Path { get; set; }

        public virtual ICollection<BookmarkBinding> Bookmarks { get; set; }
        public virtual ICollection<ChapterBinding> Chapters { get; set; }

        public TwinBook()
        {
            Bookmarks = new List<BookmarkBinding>();
            Chapters = new List<ChapterBinding>();
        }

    }
}