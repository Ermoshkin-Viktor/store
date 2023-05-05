using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Store.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithNullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(1, null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnsZero()
        {
            //пустой массив позиций ордера
            var order = new Order(1, new OrderItem[0]);
            //общее к-во книг должно быть равно 0
            Assert.Equal(0, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithEmptyItems_ReturnsZero()
        {
            //пустой массив позиций ордера
            var order = new Order(1, new OrderItem[0]);
            //общая сумма должна быть равна 0
            Assert.Equal(0m, order.TotalPrice);
        }

        [Fact]
        public void TotalCount_WithNonEmptyItems_CalculatesTotalCount()
        {
            //не пустые списки ордеров
            var order = new Order(1, new []
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Equal(3 + 5, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithNonEmptyItems_CalculatesTotalPrice()
        {
            //не пустые списки ордеров
            var order = new Order(1, new[]
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Equal(3 * 10m + 5 * 100m, order.TotalPrice);
        }

        
        //если найден: возвращает этот элемент
        [Fact]
        public void GetItem_WithExistingItem_ReturnsItem()
        {
            var order = new Order(1, new[]
           {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            var orderItem = order.GetItem(1);

            Assert.Equal(3, orderItem.Count);
        }

        //если не найден элемент: выбрасывает исключение
        [Fact]
        public void GetItem_WithNonExistingItem_ThrowsInvalidOperationException()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                order.GetItem(100);
            });
        }

        //если item есть - добавляет к-во книг
        [Fact]
        public void AddOrUpdateItem_WithExistingItem_UpdatesCount()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            var book = new Book(1, null, null, null, null, 0m);
            //добавили 10 книг
            order.AddOrUpdateItem(book, 10);

            //стало 13
            Assert.Equal(13, order.GetItem(1).Count);
        }

        //если item не существует - добавляет новый item и обновляет к-во книг
        [Fact]
        public void AddOrUpdateItem_WithNonExistingItem_AddsCount()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            var book = new Book(4, null, null, null, null, 0m);
            //добавили 10 книг
            order.AddOrUpdateItem(book, 10);

            Assert.Equal(10, order.GetItem(4).Count);//стало 10
        }

        [Fact]
        public void RemoveItem_WithExistingItem_RemovesItem()
        {
            var order = new Order(1, new[]
           {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            order.RemoveItem(1);

            Assert.Equal(1, order.Items.Count);
        }
        //с несуществующим
        [Fact]
        public void RemoveItem_WithNonExistingItem_ThrowsInvalidOperationException()
        {
            var order = new Order(1, new[]
           {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                order.RemoveItem(100);
            });
        }
    }
}
