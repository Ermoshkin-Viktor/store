using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class OrderItem
    {
        public int BookId { get; }
        //К-во книг в заказе
        public int Count { get; }
        //Цена
        public decimal Price { get; }

        public OrderItem(int bookId, int count, decimal price)
        {
            if(count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater than zero.");
            BookId = bookId;
            Count = count;
            Price = price;
        }
    }
}
