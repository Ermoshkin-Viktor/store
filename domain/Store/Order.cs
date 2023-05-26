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

        public string CellPhone { get; set; }
        //поле заполняет контроллер
        public OrderDelivery Delivery { get; set; }

        public OrderPayment Payment { get; set; }

        //Сумма количества всех экземпляров книг
        public int TotalCount => items.Sum(item => item.Count); 
        
        //Общая цена заказа с доставкой
        public decimal TotalPrice
        {                                                          //если знач =null то подставляем знач. 0m
            get { return items.Sum(item => item.Price * item.Count) + (Delivery?.Amount ?? 0m) ; }
        }

        public Order(int id, IEnumerable<OrderItem> items)
        { 
            //Мы не хотим чтобы передавался null позиций ордера
            if(items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;
            this.items = new List<OrderItem>(items);
        }

        
        public OrderItem GetItem(int bookId)
        {
            int index = items.FindIndex(item => item.BookId== bookId);
            //если элемент не найден
            if (index == -1)
                ThrowBookException("Book not found", bookId);
            
            //иначе: возвращаем
            return items[index];
        }

        //Добавление позиции к заказу
        public void AddOrUpdateItem(Book book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            //смотрим есть ли такая книга в заказе
            int index = items.FindIndex(item => item.BookId == book.Id);
            //если элемент не найден
            if (index == -1)
            {
                items.Add(new OrderItem(book.Id, count, book.Price));
            }
            else
            {
                //меняем к-во
                items[index].Count += count;
            }          
        }

        //Удаление всей товарной позиции(книга)
        public void RemoveItem(int bookId)
        {          
           //Получаем индекс
           int index = items.FindIndex(item => item.BookId == bookId);

            if(index == -1)
                ThrowBookException("Order does not contain specified item.", bookId);
             
            //Если найден то мы удаляем индекс
            items.RemoveAt(index);
        }

        private void ThrowBookException(string message, int bookId)
        {
            var exception = new InvalidOperationException(message);

            exception.Data["BookId"] = bookId;
           
            throw exception;
        }

       
    }
}
