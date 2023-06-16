using Store.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly OrderDto orderDto;

        private readonly List<OrderItem> items;

        
        public OrderItemCollection(OrderDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));
            
            this.orderDto = orderDto;
            items = orderDto.Items
                            .Select(x => OrderItem.Mapper.Map(x))
                            .ToList();
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

        public OrderItem Add(int bookId, decimal price, int count)
        {
            if (TryGet(bookId, out OrderItem orderItem))
                throw new InvalidOperationException("Book already exists");

            //Создаем orderItemDto для новой книги
            var orderItemDto = OrderItem.DtoFactory.Create(orderDto, bookId,
                                                           price, count);
            //Записываем его к родительскому объекту
            orderDto.Items.Add(orderItemDto);

            //Также записываем в список OrderItem
            orderItem = OrderItem.Mapper.Map(orderItemDto);
            items.Add(orderItem);

            return orderItem;
        }

        public void Remove(int bookId)
        {
            var index = items.FindIndex(item => item.BookId == bookId);
            if (index == -1)
                throw new InvalidOperationException("Can't find book to remove from order.");

            //Удаляем из DTO
            orderDto.Items.RemoveAt(index);
            //Удаляем из OrderItem
            items.RemoveAt(index);
        }
    }
}
