using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Store.Tests
{
    public class BookServiceTests
    {
        //Поиск по ISBN
<<<<<<< HEAD
        //[Fact]
        //public void GetAllByQuery_WithIsbn_CallsGetAllByIsbn()
        //{
        //    //заглушка для BookRepository
        //    var bookRepositoryStub = new Mock<IBookRepository>();
        //    //если будет вызываться метод GetAllByIsbn с любым строковым параметром
        //    //возратим массив типа Book c Id=1 и пустыми строками вместо названий
        //    //It.IsAny<string>() - это экспрешенс выражение(деревья выражения), т.е. код не генерируется
        //    //а выводится тип, т.е. дерево и потом сохраняется.
        //    //Т.е. мы перехватили вызов 2-х методов
        //    bookRepositoryStub.Setup(x => x.GetAllByIsbn(It.IsAny<string>()))
        //                       .Returns(new[] { new Book(1, "", "", "") });
        //    //возратим массив типа Book c Id=2 и пустыми строками вместо названий
        //    bookRepositoryStub.Setup(x => x.GetAllByTitleOrAuthor(It.IsAny<string>()))
        //                       .Returns(new[] { new Book(2, "", "", "") });
        //    //В объект BookService в качестве параметра передаем bookRepository
        //    //т.е. свойство Object нашей заглушки. Он выглядит точно так как 
        //    //BookRepository . Но вместо него выведет нужный нам массив
        //    var bookService = new BookService(bookRepositoryStub.Object);
=======
        [Fact]
        public void GetAllByQuery_WithIsbn_CallsGetAllByIsbn()
        {
            //заглушка для BookRepository
            var bookRepositoryStub = new Mock<IBookRepository>();
            //если будет вызываться метод GetAllByIsbn с любым строковым параметром
            //возратим массив типа Book c Id=1 и пустыми строками вместо названий
            //It.IsAny<string>() - это экспрешенс выражение(деревья выражения), т.е. код не генерируется
            //а выводится тип, т.е. дерево и потом сохраняется.
            //Т.е. мы перехватили вызов 2-х методов
            bookRepositoryStub.Setup(x => x.GetAllByIsbn(It.IsAny<string>()))
                               .Returns(new[] { new Book(1, "", "", "", "", 0m) });
            //возратим массив типа Book c Id=2 и пустыми строками вместо названий
            bookRepositoryStub.Setup(x => x.GetAllByTitleOrAuthor(It.IsAny<string>()))
                               .Returns(new[] { new Book(2, "", "", "", "", 0m) });
            //В объект BookService в качестве параметра передаем bookRepository
            //т.е. свойство Object нашей заглушки. Он выглядит точно так как 
            //BookRepository . Но вместо него выведет нужный нам массив
            var bookService = new BookService(bookRepositoryStub.Object);
>>>>>>> 43380b0fad0b7bb0529faa5f988190c135e7b4e3

        //    var validIsbn = "ISBN 12345-67890";

        //    var actual = bookService.GetAllByQuery(validIsbn);
        //    //Проверяем значение Id (1 или 2)
        //    //Equal(ожидаемое значение, передаваемое значение)
        //    Assert.Collection(actual, book => Assert.Equal(1, book.Id));
        //}

        //Поиск по автору
        [Fact]
        public void GetAllByQuery_WithAuthor_CallsGetAllByTitleOrAuthor()
        {
            //заглушка для BookRepository
            var bookRepositoryStub = new Mock<IBookRepository>();
            //если будет вызываться метод GetAllByIsbn с любым строковым параметром
            //возратим массив типа Book c Id=1 и пустыми строками вместо названий
            //It.IsAny<string>() - это экспрешенс выражение(деревья выражения), т.е. код не генерируется
            //а выводится тип, т.е. дерево и потом сохраняется.
            //Т.е. мы перехватили вызов 2-х методов
<<<<<<< HEAD
            bookRepositoryStub.Setup(x => x.GetAllByIsbn("Knuth"))
                               .Returns(new[] { new Book(1, "", "", "") });
=======
            bookRepositoryStub.Setup(x => x.GetAllByIsbn(It.IsAny<string>()))
                               .Returns(new[] { new Book(1, "", "", "", "", 0m) });
>>>>>>> 43380b0fad0b7bb0529faa5f988190c135e7b4e3
            //возратим массив типа Book c Id=2 и пустыми строками вместо названий
            bookRepositoryStub.Setup(x => x.GetAllByTitleOrAuthor(It.IsAny<string>()))
                               .Returns(new[] { new Book(2, "", "", "", "", 0m) });
            //В объект BookService в качестве параметра передаем bookRepository
            //т.е. свойство Object нашей заглушки. Он выглядит точно так как 
            //BookRepository . Но вместо него выведет нужный нам массив
            var bookService = new BookService(bookRepositoryStub.Object);

            var author = "Knuth";

            var actual = bookService.GetAllByQuery(author);
            //Проверяем значение Id (1 или 2)
            //Equal(ожидаемое значение, передаваемое значение)
            Assert.Collection(actual, book => Assert.Equal(2, book.Id));
        }

        //Второй вариант тестов 
        [Fact]
        public void GetAllByQuery_WithIsbn_CallsGetAllByIsbn()
        {
            const int idOfIsbnSearch = 1;
            const int idOfAuthorSearch = 2;

            var bookRepository = new StubBookRepository();
            //присваиваем значение свойству
            bookRepository.ResultOfGetAllByIsbn = new[]
            {
                new Book(idOfIsbnSearch, "", "", "")
            };
            //присваиваем значение свойству
            bookRepository.ResultOfGetAllByTitleOrAuthor = new[]
            {
                new Book(idOfAuthorSearch, "", "", "")
            };

            var bookService = new BookService(bookRepository);
            //получаем книги у сервиса
            var books = bookService.GetAllByQuery("ISBN 12345-67890");
            //проверка условий на коллекции и проверка условий на равенство
            Assert.Collection(books, book => Assert.Equal(idOfIsbnSearch, book.Id));
        }

        [Fact]
        public void GetAllByQuery_WithTitle_CallsGetAllByTitleOrAuthor()
        {
            const int idOfIsbnSearch = 1;
            const int idOfAuthorSearch = 2;

            var bookRepository = new StubBookRepository();

            bookRepository.ResultOfGetAllByIsbn = new[]
            {
                new Book(idOfIsbnSearch, "", "", "")
            };

            bookRepository.ResultOfGetAllByTitleOrAuthor = new[]
            {
                new Book(idOfAuthorSearch, "", "", "")
            };

            var bookService = new BookService(bookRepository);
            //получаем книги у сервиса
            var books = bookService.GetAllByQuery("Programming");
            //проверка условий на коллекции и проверка условий на равенство
            //смотрим значение id 1 или 2
            Assert.Collection(books, book => Assert.Equal(idOfAuthorSearch, book.Id));
        }
    }
}
