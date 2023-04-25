using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepository
    {
        Book GetById(int id);

        //метод возвращающий массив книг по ISBN
        Book[] GetAllByIsbn(string isbn);
        //метод возвращающий массив книг по названию
        Book[] GetAllByTitleOrAuthor(string titleOrAuthor);

        Book[] GetAllByIds(IEnumerable<int> bookIds);
    }
}
