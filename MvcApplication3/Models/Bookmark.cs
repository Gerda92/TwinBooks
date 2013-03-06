using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyReading.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }
        public virtual Book InBook { get; set; }
        public string BookmarkId { get; set; }
        public int Order { get; set; }
    }
}