using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly List<OrderItem> items;

        //IEnumerable<OrderItem> items - итератор позиций в заказе
        public OrderItemCollection(IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            //из итератора позиции заказа заносим в список
            this.items = new List<OrderItem>(items);
        }
        //возвращает количество элементов
        public int Count => items.Count;

        //итерация по коллекциям
        public IEnumerator<OrderItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            //возвращает тип object
            return (items as IEnumerable).GetEnumerator();
        }

        //проверка- на наличие элемента (если нет-исключение)
        public OrderItem Get(int bookId)
        {
            if(TryGet(bookId, out OrderItem orderItem)) 
                return orderItem;

            throw new InvalidOperationException("Book not found.");
        }
        //проверка- на наличие элемента
        public bool TryGet(int bookId, out OrderItem orderItem)
        {
            var index = items.FindIndex(item => item.BookId== bookId);
            if(index == -1)
            {
                orderItem = null;
                return false;
            }

            orderItem = items[index]; 
            return true;
        }

        public OrderItem Add(int bookId, decimal bookPrice, int count)
        {
            if (TryGet(bookId, out OrderItem orderItem))
                throw new InvalidOperationException("Book alread");

            orderItem = new OrderItem(bookId, bookPrice, count);
            items.Add(orderItem);

            return orderItem;
        }

        public void Remove(int bookId)
        {
            items.Remove(Get(bookId));
        }
    }
}
