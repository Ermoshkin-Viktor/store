using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class Order
    {
        public int Id { get; }

        private List<OrderItem> items;
        //Нельзя изменять
        public IReadOnlyCollection<OrderItem> Items 
        {
            get { return items; } 
        }

        public int TotalCount
        {
            //Сумма количества всех экземпляров книг
            get { return items.Sum(item => item.Count); }
        }

        //Общая цена заказа
        public decimal TotalPrice
        {
            get { return items.Sum(item => item.Price * item.Count); }
        }
        public Order(int id, IEnumerable<OrderItem> items)
        { 
            //Мы не хотим чтобы передавался null позиций ордера
            if(items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;
            this.items = new List<OrderItem>(items);
        }

        //Добавление позиции к заказу
        public void AddItem(Book book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            //смотрим есть ли такая книга в заказе
            var item = items.SingleOrDefault(x => x.BookId == book.Id);
            //если такой книги в заказе нет то добавляем ее
            if(item == null)
            {
                items.Add(new OrderItem(book.Id, count, book.Price));
            }
            else
            {
                //если такая книжка в заказе есть, то старую запись удаляем,
                //а новую добавляем уеличивая количество
                items.Remove(item);
                items.Add(new OrderItem(book.Id, item.Count + count, book.Price));
            }
        }
    }
}
