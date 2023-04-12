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

            var validIsbn = "ISBN 12345-67890";

            var actual = bookService.GetAllByQuery(validIsbn);
            //Проверяем значение Id (1 или 2)
            //Equal(ожидаемое значение, передаваемое значение)
            Assert.Collection(actual, book => Assert.Equal(1, book.Id));
        }

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
            bookRepositoryStub.Setup(x => x.GetAllByIsbn(It.IsAny<string>()))
                               .Returns(new[] { new Book(1, "", "", "", "", 0m) });
            //возратим массив типа Book c Id=2 и пустыми строками вместо названий
            bookRepositoryStub.Setup(x => x.GetAllByTitleOrAuthor(It.IsAny<string>()))
                               .Returns(new[] { new Book(2, "", "", "", "", 0m) });
            //В объект BookService в качестве параметра передаем bookRepository
            //т.е. свойство Object нашей заглушки. Он выглядит точно так как 
            //BookRepository . Но вместо него выведет нужный нам массив
            var bookService = new BookService(bookRepositoryStub.Object);

            var invalidIsbn = "Knuth";

            var actual = bookService.GetAllByQuery(invalidIsbn);
            //Проверяем значение Id (1 или 2)
            //Equal(ожидаемое значение, передаваемое значение)
            Assert.Collection(actual, book => Assert.Equal(2, book.Id));
        }
    }
}
