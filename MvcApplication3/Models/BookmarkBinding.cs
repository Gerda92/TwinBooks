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
        public int BookId1 { get; set; }
        public int BookId2 { get; set; }
        public string BookmarkId1 { get; set; }
        public string BookmarkId2 { get; set; }
        public int Order1 { get; set; }
        public int Order2 { get; set; }
        public DateTime CreatedAt { get; set; }

        public BookmarkBinding () 
        {
            CreatedAt = DateTime.Now;
        }
    }
}