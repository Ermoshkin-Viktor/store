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
        Order Create();
        //Загрузка заказа
        Order GetById(int id);
        //обновление
        void Update(Order order);
    }
}
