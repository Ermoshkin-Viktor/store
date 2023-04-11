using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepository
    {
        //метод возвращающий массив книг по ISBN
        Book[] GetAllByIsbn(string isbn);
        //метод возвращающий массив книг по названию
        Book[] GetAllByTitleOrAuthor(string titleOrAuthor);
    }
}
