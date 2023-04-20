using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Memory
{
    public class OrderRepository : IOrderRepository
    {
        private readonly List<Order> orders = new List<Order>();
        //Создаем заказ
        public Order Create()
        {
            //id нового заказа
            int nextId = orders.Count +1;
            //передаем id и пустой массив
            var order = new Order(nextId, new OrderItem[0]);
            //добавляем
            orders.Add(order);

            return order;
        }

        public Order GetById(int id)
        {
            return orders.Single(order => order.Id == id);
        }

        public void Update(Order order)
        {
            ;
        }
    }
}
