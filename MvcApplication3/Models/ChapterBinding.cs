using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyReading.Models
{
    public class ChapterBinding
    {
        [Key]
        public int Id { get; set; }
        public virtual Chapter ChapterOne { get; set; }
        public virtual Chapter ChapterTwo { get; set; }
    }
}