using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepository
    {
        Task<Book[]> GetAllByIsbnAsync(string isbn);

        Task<Book[]> GetAllByTitleOrAuthorAsync(string titleOrAuthor);

        Task<Book> GetByIdAsync(int id);

        Task<Book[]> GetAllByIdsAsync(IEnumerable<int> bookIds);
    }
}
