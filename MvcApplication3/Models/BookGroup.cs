using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyReading.Models
{
    public class BookGroup
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public BookGroup() {
            Books = new List<Book>();
        }
    }
}