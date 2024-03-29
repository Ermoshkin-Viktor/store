﻿using System;
using System.Collections.Generic;


namespace Store
{
    //Патерн Value Object (объект значения) (нет своего идентификатора)
    public class OrderDelivery
    {
        public string UniqueCode { get; }

        public string Description { get; }
        //стоимость доставки
        public decimal Price { get; }
        //для хранения информации (нельзя менять)
        public IReadOnlyDictionary<string, string> Parameters { get; } = 
                 new Dictionary<string, string>();

        public OrderDelivery(string uniqueCode, string description, decimal amount,
                             IReadOnlyDictionary<string, string> parameters)
        {
            if(string.IsNullOrWhiteSpace(uniqueCode))
                throw new ArgumentException(nameof(uniqueCode));

            if(string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));

            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            UniqueCode = uniqueCode ;
            Description = description ;
            Price = amount ;
            Parameters = parameters ;
        }
    }
}
