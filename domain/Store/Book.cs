using System;
namespace Store
    //DDD(Entity- сущность, Value Objeckt- область значений)
{
    public class Book//сущность
    {
        public int Id { get;  }
        //название книги
        public string Title { get;  }
        public Book(int id, string title)
        {
            Id = id;
            Title = title;         
        }
    }
}
