using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests
{
    //Класс-заглушка(перенаправляет выполнение программы) 
    public class StubBookRepository : IBookRepository
    {
        public Book[] ResultOfGetAllByIsbn { get; set; }

        public Book[] ResultOfGetAllByTitleOrAuthor { get; set; }
        
        Book[] IBookRepository.GetAllByIsbn(string isbn)
        {
            return ResultOfGetAllByIsbn;
        }

        Book[] IBookRepository.GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            return ResultOfGetAllByTitleOrAuthor;
        }
    }
}
