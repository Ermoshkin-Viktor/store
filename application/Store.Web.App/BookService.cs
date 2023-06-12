using Store.Web.App;
using System.Collections.Generic;
using System.Linq;


namespace Store //предметная область
{//Служба
    //Новый уровень косвенности (прикладной уровень)
    //Сервис получает параметры из представления и выводит модель
    public class BookService
    {
        private readonly IBookRepository bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public IReadOnlyCollection<BookModel> GetAllByQuery(string query)
        {
            var books = Book.IsIsbn(query)
                ? bookRepository.GetAllByIsbn(query)
                : bookRepository.GetAllByTitleOrAuthor(query);

            return books.Select(Map)
                .ToArray();
        }

        public BookModel GetById(int id)
        {
            var book = bookRepository.GetById(id);
            return Map(book);
        }

        private BookModel Map(Book book)
        {
            return new BookModel
            {
                Id= book.Id,
                Isbn= book.Isbn,
                Title= book.Title,
                Author= book.Author,
                Description= book.Description,
                Price= book.Price,
            };
        }
    }
}
