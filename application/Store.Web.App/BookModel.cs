using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App
{
    public class BookModel
    {
        public int Id { get; set; }

        public string Isbn { get; set; }

        public string Author { get; set; }
        //название книги
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
