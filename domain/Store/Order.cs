using Store.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class Order
    { 
        //Создаем объект один раз и не можем пересоздать
        //А поля объекта изменять можем
        private readonly OrderDto dto;

        public int Id => dto.Id;

        public string CellPhone
        {
            get => dto.CellPhone;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(CellPhone));
                
                dto.CellPhone = value;
            }
        }
        
        public OrderDelivery Delivery
        {
            get
            {
                if(dto.DeliveryUnicueCode == null)
                    return null;
                //Собираем объект OrderDelivery
                return new OrderDelivery(
                    dto.DeliveryUnicueCode,
                    dto.DeliveryDescription,
                    dto.DeliveryPrice,
                    dto.DeliveryParameters);
            }
            //при изменении null присвоить нельзя
            set
            {
                if(value == null)
                    throw new ArgumentException(nameof(Delivery));

                dto.DeliveryUnicueCode = value.UniqueCode;
                dto.DeliveryDescription = value.Description;
                dto.DeliveryPrice = value.Price;
                dto.DeliveryParameters = value.Parameters
                            .ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public OrderPayment Payment
        {
            get
            {
                if (dto.PaymentServiceName == null)
                    return null;
                //Собираем объект OrderDelivery
                return new OrderPayment(
                    dto.PaymentServiceName,
                    dto.PaymentDescription,
                    dto.PaymentParameters);
            }
            //при изменении null присвоить нельзя
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Delivery));

                dto.PaymentServiceName = value.UniqueCode;
                dto.PaymentDescription = value.Description;
                dto.PaymentParameters = value.Parameters
                            .ToDictionary(p => p.Key, p => p.Value);
            }
        }
        //Позиция заказа
        public OrderItemCollection Items { get; }

        //Сумма количества всех экземпляров книг
        public int TotalCount => Items.Sum(item => item.Count); 
        
        //Общая цена заказа с доставкой
        public decimal TotalPrice
        {                                                          //если знач =null то подставляем знач. 0m
            get { return Items.Sum(item => item.Price * item.Count) + (Delivery?.Price ?? 0m) ; }
        }

        public Order(OrderDto dto)
        {
            this.dto = dto;
            Items = new OrderItemCollection(dto);
        }
        
        public static class DtoFactory
        {
            public static OrderDto Create() => new OrderDto();
        }

        public static class Mapper
        {
            public static Order Map(OrderDto dto) => new Order(dto);

            public static OrderDto Map(Order domain) => domain.dto;
        }
    }
}
