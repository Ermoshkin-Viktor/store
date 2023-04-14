using System.Collections.Generic;

namespace Store.Web.Models
{
    public class Cart
    {                        
        //Словарь у которого ключи(Id) и значения(количество) целые числа
        public IDictionary<int, int> Items { get; set; } = new Dictionary<int, int>();

        //Сумма товаров хранящихся в корзине
        public decimal Amount { get; set; } 
    }
}
