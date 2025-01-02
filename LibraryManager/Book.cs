using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager
{
    internal class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }    

        public Book(string title, string author, string genre, int year)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
        }

        public override string ToString()
        {
            return $"{Title},{Author},{Genre},{Year}";
        }

        public static Book FromString(string bookData)
        {
            var parts = bookData.Split(",");
            if (parts.Length != 4)
                throw new FormatException("A könyv adatainak formátuma nem megfelelő");
            return new Book(parts[0], parts[1], parts[2], int.Parse(parts[3]));
        }
    }
}
