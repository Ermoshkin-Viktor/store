﻿using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using Store.Messages;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.Web.App
{
    public class OrderService
    {
        //Отсюда мы обращаемся к репозиторию книг
        private readonly IBookRepository bookRepository;
        //Отсюда мы обращаемся к репозиторию заказов
        private readonly IOrderRepository orderRepository;
        //Рассылка уведомлений
        private readonly INotificationService notificationService;
        //Доступ к текущей сессии
        private readonly IHttpContextAccessor httpContextAccessor;

        //Св-во возвращающее текущую сессию из текущего HttpContext
        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public OrderService(IBookRepository bookRepository, 
                            IOrderRepository orderRepository,  
                            INotificationService notificationService, 
                            IHttpContextAccessor httpContextAccessor)
        {
            this.bookRepository = bookRepository  ;
            this.orderRepository = orderRepository ;
            this.notificationService = notificationService ;
            this.httpContextAccessor = httpContextAccessor ;
        }

        public bool TryGetModel(out OrderModel model)
        {
            if (TryGetOrder(out Order order))
            {
                model = Map(order);
                return true;
            }

            model = null;
            return false;
        }

        internal bool TryGetOrder(out Order order)
        {
            if (Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
                return true;
            }

            order = null;
            return false;
        }

        internal OrderModel Map(Order order)
        {
            var books = GetBooks(order);
            var items = from item in order.Items
                        join book in books on item.BookId equals book.Id
                        select new OrderItemModel
                        {
                            BookId = book.Id,
                            Title = book.Title,
                            Author = book.Author,
                            Price = item.Price,
                            Count = item.Count,
                        };

            return new OrderModel
            {
                Id = order.Id,
                Items = items.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                CellPhone = order.CellPhone,
                DeliveryDescription = order.Delivery?.Description,
                PaymentDescription = order.Payment?.Description
            };
        }

        internal IEnumerable<Book> GetBooks(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);

            return bookRepository.GetAllByIds(bookIds);
        }

        public OrderModel AddBook(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to add");

            if (!TryGetOrder(out Order order))
                //если такого ордера нет то мы его создаем
                order = orderRepository.Create();

            AddOrUpdateBook(order, bookId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal void AddOrUpdateBook(Order order, int bookId, int count)
        {
            var book = bookRepository.GetById(bookId);
            if (order.Items.TryGet(bookId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(book.Id, book.Price, count);
        }

        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        public OrderModel UpdateBook(int bookId, int count)
        {
            var order = GetOrder();
            order.Items.Get(bookId).Count = count;

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public OrderModel RemoveBook(int bookId)
        {
            var order = GetOrder();
            order.Items.Remove(bookId);

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public Order GetOrder()
        {
            if (TryGetOrder(out Order order))
                return order;

            throw new InvalidOperationException("Empty session.");
        }

        public OrderModel SendConfirmation(string cellPhone)
        {
            var order = GetOrder();
            var model = Map(order);

            if (TryFormatPhone(cellPhone, out string formattedPhone))
            {
                var confirmationCode = 1111; // todo: random.Next(1000, 10000) = 1000, 1001, ..., 9998, 9999
                model.CellPhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                notificationService.SendConfirmationCode(formattedPhone, confirmationCode);
            }
            else
                model.Errors["cellPhone"] = "Номер телефона не соответствует формату +79876543210";

            return model;
        }

        private readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        internal bool TryFormatPhone(string cellPhone, out string formattedPhone)
        {
            try
            {
                var phoneNumber = phoneNumberUtil.Parse(cellPhone, "ru");
                formattedPhone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                return true;
            }
            catch (NumberParseException)
            {
                formattedPhone = null;
                return false;
            }
        }

        public OrderModel ConfirmCellPhone(string cellPhone, int confirmationCode)
        {
            int? storedCode = Session.GetInt32(cellPhone);
            var model = new OrderModel();

            if (storedCode == null)
            {
                model.Errors["cellPhone"] = "Что-то случилось. Попробуйте получить код ещё раз.";
                return model;
            }

            if (storedCode != confirmationCode)
            {
                model.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте ещё раз.";
                return model;
            }

            var order = GetOrder();
            order.CellPhone = cellPhone;
            orderRepository.Update(order);

            Session.Remove(cellPhone);

            return Map(order);
        }

        public OrderModel SetDelivery(OrderDelivery delivery)
        {
            var order = GetOrder();
            order.Delivery = delivery;
            orderRepository.Update(order);

            return Map(order);
        }

        public OrderModel SetPayment(OrderPayment payment)
        {
            var order = GetOrder();
            order.Payment = payment;
            orderRepository.Update(order);
            Session.RemoveCart();

            return Map(order);
        }
        ////Проверка на наличие модели
        //public bool TryGetModel(out OrderModel model)
        //{
        //    if(TryGetOrder(out Order order))
        //    {
        //        model = Map(order);
        //        return true;
        //    }
        //    model= null;
        //    return false;
        //}



        //internal OrderModel Map(Order order)
        //{
        //    var books = GetBooks(order);
        //    var items = from item in order.Items
        //                join book in books on item.BookId equals book.Id
        //                select new OrderItemModel
        //                {
        //                    BookId = book.Id,
        //                    Title= book.Title,
        //                    Author= book.Author,
        //                    Count = item.Count,
        //                    Price = item.Price,                   
        //                };
        //    return new OrderModel
        //    {
        //        Id= order.Id,
        //        Items= items.ToArray(),
        //        TotalCount= order.TotalCount,
        //        TotalPrice= order.TotalPrice,
        //        CellPhone= order.CellPhone,
        //        DeliveryDescription= order.Delivery?.Description,
        //        PaymentDescription= order.Payment?.Description,
        //    };
        //}

        ////Проверка- есть ли такой заказ
        //internal bool TryGetOrder(out Order order)
        //{
        //    //по номеру в текущей корзине
        //    if(Session.TryGetCart(out Cart cart))
        //    {
        //        order = orderRepository.GetById(cart.OrderId) ;
        //        return true ;
        //    }

        //    order= null ;
        //    return false ;
        //}

        //internal IEnumerable<Book> GetBooks(Order order)
        //{
        //    var bookIds = order.Items.Select(item => item.BookId);
        //    return bookRepository.GetAllByIds(bookIds) ;
        //}
        ////Добавляем книгу в заказ
        //public OrderModel AddBook(int bookId, int count)
        //{
        //    if (count < 1)
        //        throw new InvalidOperationException("Too few books to add");

        //    if(!TryGetOrder(out Order order))
        //    {
        //        AddOrUpdateBook(order, bookId, count);
        //        UpdateSession(order);
        //    }

        //    return Map(order);
        //}
        ////обновляем добавку книги
        //public OrderModel UpdateBook(int bookId, int count)
        //{
        //    var order = GetOrder();
        //    order.Items.Get(bookId).Count = count;

        //    orderRepository.Update(order);
        //    UpdateSession(order);

        //    return Map(order);
        //}
        ////Удаляем книгу
        //public OrderModel RemoveBook(int bookId)
        //{
        //    var order = GetOrder();
        //    order.Items.Remove(bookId);

        //    orderRepository.Update(order); 
        //    return Map(order);
        //}

        //public Order GetOrder()
        //{
        //    if(TryGetOrder(out Order order))
        //    {
        //        return order;
        //    }
        //    //Если такой книги нет в заказе- ошибка
        //    throw new InvalidOperationException("Empty session");
        //}

        //internal void AddOrUpdateBook(Order order, int bookId, int count)
        //{
        //    var book = bookRepository.GetById(bookId);
        //    if (order.Items.TryGet(bookId, out OrderItem orderItem))
        //        orderItem.Count += count;
        //    else
        //        order.Items.Add(bookId, book.Price, count);
        //}

        //internal void UpdateSession(Order order)
        //{
        //    var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
        //    Session.Set(cart);
        //}
        ////Отправляем код подтверждения телефона
        //public OrderModel SendConfirmation(string cellPhone)
        //{
        //    var order = GetOrder();
        //    var model = Map(order);

        //    if (TryFormatPhone(cellPhone, out string formatedPhone))
        //    {
        //        var confirmationCode = 1111;// todo: random.Next(1000, 10000) = 1000, 1001, ..., 9998, 9999
        //        model.CellPhone = formatedPhone;
        //        Session.SetInt32(formatedPhone, confirmationCode);
        //        notificationService.SendConfirmationCode(formatedPhone, confirmationCode);
        //    }
        //    else
        //        model.Errors["cellPhone"] = "Номер телефона не соответсвует" +
        //            "формату +79876543210";

        //    return model;
        //}

        //private readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        //internal bool TryFormatPhone(string cellPhone, out string formattedPhone)
        //{
        //    try
        //    {
        //        var phoneNumber = phoneNumberUtil.Parse(cellPhone, "ru");
        //        formattedPhone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
        //        return true;
        //    }
        //    catch (NumberParseException)
        //    {
        //        formattedPhone = null;
        //        return false;
        //    }
        //}

        ////подтверждение номера телефона
        //public OrderModel ConfirmCellPhone(string cellPhone, int confirmationCode)
        //{
        //    int? storedCode = Session.GetInt32(cellPhone);
        //    var model = new OrderModel();

        //    if (storedCode == null)
        //    {
        //        model.Errors["cellPhone"] = "Что-то случилось. Попробуйте получить код ещё раз.";
        //        return model;
        //    }
        //    //сравниваем введенный код с правильным
        //    if (storedCode != confirmationCode)
        //    {
        //        model.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте ещё раз.";
        //        return model;
        //    }

        //    var order = GetOrder();
        //    order.CellPhone = cellPhone;//присваиваем номер телефона
        //    orderRepository.Update(order);//сохраняем заказ

        //    Session.Remove(cellPhone);//удаляем номер телефона из сессии

        //    return Map(order);
        //}
        ////Устанавливаем информацию о доставке
        //public OrderModel SetDelivery(OrderDelivery delivery)
        //{
        //    var order = GetOrder();
        //    order.Delivery = delivery;
        //    orderRepository.Update(order);

        //    return Map(order);
        //}
        ////Устанавливаем информацию о платеже
        //public OrderModel SetPayment(OrderPayment payment)
        //{
        //    var order = GetOrder();
        //    order.Payment = payment;
        //    orderRepository.Update(order);
        //    Session.RemoveCart();

        //    return Map(order);
        //}
    }
}