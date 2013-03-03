using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyReading.Models
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }
        public string ChapterId { get; set; }
        public virtual Book InBook { get; set; }
        public int Order { get; set; }

        public virtual Chapter Parent { get; set; }
        public virtual ICollection<Chapter> Children { get; set; }

        public Chapter()
        {
            Children = new List<Chapter>();
        }
    }
}