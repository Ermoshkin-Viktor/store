using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IOrderRepository
    {
        //Создание заказа
        Task<Order> CreateAsync();
        //Загрузка заказа
        Task<Order> GetByIdAsync(int id);
        //обновление
        Task UpdateAsync(Order order);
    }
}
