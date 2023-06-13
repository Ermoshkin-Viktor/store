using Store.Data;
using System;
using System.Text.RegularExpressions;

namespace Store
    //DDD(Entity- сущность, Value Objeckt- область значений)
{
    public class Book//сущность
    {
        private readonly BookDto dto;

        public int Id { get { return dto.Id; } }

        public string Isbn 
        { 
            get => dto.Isbn;
            set
            {
                if (TryFormatIsbn(value, out string formatedIsbn))
                    dto.Isbn = formatedIsbn;

                throw new ArgumentException(nameof(Isbn));               
            }
        }

        public string Author 
        { 
            get => dto.Author;
            set=> dto.Author = value;
        }
        //название книги
        public string Title 
        { 
            get => dto.Title;
            set 
            {
                //если название не записано в поиске, то ошибка
                if(string.IsNullOrWhiteSpace(value)) 
                    throw new ArgumentNullException(nameof(Title));

                dto.Title = value.Trim();
            }
        }

        public string Description 
        {
            get => dto.Description;
            set=> dto.Description = value;
        }

        public decimal Price 
        {
            get => dto.Price;
            set=> dto.Price = value;
        }

        internal Book(BookDto dto)
        {
            this.dto = dto;
        }

        public static bool TryFormatIsbn(string isbn, out string formatedIsbn)
        {
            if (isbn == null)
            {
                formatedIsbn= null;
                return false;
            }
            
            formatedIsbn = isbn.Replace(" ", "")
                .Replace("-", "")
                .ToUpper();
            return Regex.IsMatch(formatedIsbn, @"^ISBN\d{10}(\d{3})?$");//возратит true если подстрока совпадет с шаблоном 
        }

        public static bool IsIsbn(string isbn) =>
              TryFormatIsbn(isbn, out _);

        //Создает DTO объекты
        //Проверяет правильность параметров
        public static class DtoFactory
        {
            public static BookDto Create(string isbn,
                                         string author, 
                                         string title,
                                         string description,
                                         decimal price)
            {
                if(TryFormatIsbn(isbn, out string formatedIsbn))
                    isbn = formatedIsbn;
                else
                throw new ArgumentException(nameof(isbn));

                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentNullException(nameof(title));

                return new BookDto
                {
                    Isbn= isbn,
                    //Удаляам пробелы
                    Author= author?.Trim(), 
                    Title= title.Trim(),
                    Description= description?.Trim(),
                    Price= price
                };
            }
        }

        public static class Mapper
        {
            //Отображает DTO на сущность Book (вызывает конструктор)
            public static Book Map(BookDto dto) => new Book(dto);

            //Извлекает DTO из доменной сущности Book
            public static BookDto  Map(Book domain) => domain.dto;
        }
    }
}
