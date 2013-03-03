using eBdb.EpubReader;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EasyReading.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Language { get; set; }

        // path to source (.epub) file
        public string SourcePath { get; set; }
        // path to prepared html file
        public string Path { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }

        // The same book in different languages or variants is one group
        public BookGroup Group { get; set; }

        public Book()
        {
            Chapters = new List<Chapter>();
        }

    }
}