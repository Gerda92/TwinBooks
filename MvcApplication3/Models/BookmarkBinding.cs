using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyReading.Models
{
    public class BookmarkBinding
    {
        [Key]
        public int Id { get; set; }
        public virtual Bookmark Bookmark1 { get; set; }
        public virtual Bookmark Bookmark2 { get; set; }
        public int Type { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }

        public BookmarkBinding () 
        {
            Active = true;
            CreatedAt = DateTime.Now;
        }
    }
}