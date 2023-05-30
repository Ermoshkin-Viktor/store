

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Contractors;
using Store.Messages;
using Store.Web.Contractors;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        //Список всех зарегистрированных сервисов доставки
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;
        private readonly IEnumerable<IWebContractorService> webContractorServices;
        private readonly INotificationService notificationService;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository,
                         IEnumerable<IDeliveryService> deliveryServices,
                         IEnumerable<IPaymentService> paymentServices,
                         IEnumerable<IWebContractorService> webContractorServices,
                         INotificationService notificationService)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
            this.webContractorServices = webContractorServices;
            this.notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //если в корзине есть книги
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                //загружаем ордер(подгружаем нужные книги)
                var order = orderRepository.GetById(cart.OrderId);
                //создаем модель
                OrderModel model = Map(order);
                return View(model);
            }
            return View("Empty");
        }

        private OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);
            var books = bookRepository.GetAllByIds(bookIds);
            var itemModels = from item in order.Items
                             join book in books on item.BookId equals book.Id
                             select new OrderItemModel
                             {
                                 BookId = book.Id,
                                 Title = book.Title,
                                 Author = book.Author,
                                 Price = item.Price,
                                 Count = item.Count
                             };
            return new OrderModel
            {
                Id = order.Id,
                Items = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
            };
        }

        [HttpPost]
        public IActionResult AddItem(int bookId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            //получаем книгу из репозитория
            var book = bookRepository.GetById(bookId);

            order.AddOrUpdateItem(book, count);

            //обновляем в базе
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        //обновление
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            //получаем книгу, увеличиваем к-во          
            order.GetItem(bookId).Count = count;
            //обновляем в базе
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        private (Order order, Cart cart) GetOrCreateOrderAndCart()
        {
            Order order;
            //Если корзина с таким id есть
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                //Если нет, создаем новый заказ
                order = orderRepository.Create();
                //и новую корзину
                cart = new Cart(order.Id);
            }
            return (order, cart);
        }

        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepository.Update(order);
            //обновляем к-во и цену
            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
            //сохраняем сессию
            HttpContext.Session.Set(cart);
        }

        [HttpPost]
        public IActionResult RemoveItem(int bookid)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.RemoveItem(bookid);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string cellPhone)
        {
            var order = orderRepository.GetById(id);
            var model = Map(order);

            if (!IsValidCellPhone(cellPhone))
            {
                model.Errors["cellPhone"] = "Номер телефона не соответствует формату +79876543210";
                return View("Index", model);
            }

            int code = 1111;//random.Next(1000, 10000)
            HttpContext.Session.SetInt32(cellPhone, code);
            notificationService.SendConfirmationCode(cellPhone, code);

            return View("Confirmation", 
                         new ConfirmationModel 
                         { 
                             OrderId = id, 
                             CellPhone = cellPhone 
                         });
        }

        private bool IsValidCellPhone(string cellPhone)
        {
            if (cellPhone == null)
            {
                return false;
            }
            cellPhone = cellPhone.Replace(" ", "")
                                .Replace("-", "");

            return Regex.IsMatch(cellPhone, @"^\+?\d{11}$");
        }

        [HttpPost]
        public IActionResult Confirmate(int id, string cellPhone, int code)
        {
            //загружаем код из сессии с предыдущего шага
            int? storedCode = HttpContext.Session.GetInt32(cellPhone); 
            //если там ничего не хранилось
            if(storedCode == null)
            {
                return View("Confirmation",
                        new ConfirmationModel
                        {
                            OrderId = id,
                            CellPhone = cellPhone,
                            Errors = new Dictionary<string, string> 
                            {
                                { "code", "Пустой код, повторите отправку "}
                            },
                        }) ; 
            }

            if(storedCode != code)
            {
                return View("Confirmation",
                        new ConfirmationModel
                        {
                            OrderId = id,
                            CellPhone = cellPhone,
                            Errors = new Dictionary<string, string>
                            {
                                { "code", "Отличается от отправленного"}
                            },
                        });
            }

            // сохранить информацию о номере телефона
            var order = orderRepository.GetById(id);
            order.CellPhone = cellPhone;
            //сохранить в базе данных
            orderRepository.Update(order);

            //удаляем из сессии номер телефона
            HttpContext.Session.Remove(cellPhone);

            var model = new DeliveryModel
            {
                OrderId= id,
                //словарь- ключ:поле UniqueCode, а значение:название
                Methods = deliveryServices.ToDictionary(service => service.UniqueCode,
                                                       service => service.Title),
            };

            return View("DeliveryMethod", model);
        }

        //Загрузка первой формы
        [HttpPost]
        public IActionResult StartDelivery(int id, string uniqueCode)
        {
            //Загружаем необходимый нам deliveryService у которого
            //уникальный код совпадает с переданным
            var deliveryService = deliveryServices
                    .Single(service => service.UniqueCode == uniqueCode);
            //загружаем ордер
            var order = orderRepository.GetById(id);
            //создаем первую форму
            var form = deliveryService.CreateForm(order);

            return View("DeliveryStep", form);
        }

        //Загрузка последующей формы
        [HttpPost]
        public IActionResult NextDelivery(int id, string uniqueCode,
                   int step, Dictionary<string, string> values)
        {
            //Загружаем необходимый нам deliveryService у которого
            //уникальный код совпадает с переданным
            var deliveryService = deliveryServices
                    .Single(service => service.UniqueCode == uniqueCode);
            var form = deliveryService.MoveNextForm(id, step, values);

            //если форма финальная
            if(form.IsFinal)
            {
                //переход к следующему шагу(сохраняем в заказе)
                var order = orderRepository.GetById(id);
                order.Delivery = deliveryService.GetDelivery(form);
                orderRepository.Update(order);

                var model = new DeliveryModel
                {
                    OrderId = id,
                    //словарь- ключ:поле UniqueCode, а значение:название
                    Methods = paymentServices.ToDictionary(service => service.UniqueCode,
                                                      service => service.Title),
                };

                return View("PaymentMethod", model);
            }
            //иначе продолжаем
            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult StartPayment(int id, string uniqueCode)
        {
            //получаем единственную службу с таким именем и она точно есть
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepository.GetById(id);

            var form = paymentService.CreateForm(order);
            
            //мы точно не знаем есть ли такая служба
            var webContractorService = webContractorServices.SingleOrDefault(service => service.UniqueCode == uniqueCode);
            if(webContractorService != null)
            {
                return Redirect(webContractorService.GetUri);
            }
            return View("PaymentStep", form);
        }

        [HttpPost]
        public IActionResult NextPayment(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);

            var form = paymentService.MoveNextForm(id, step, values);

            if (form.IsFinal)
            {
                var order = orderRepository.GetById(id);
                order.Payment = paymentService.GetPayment(form);
                orderRepository.Update(order);

                return View("Finish");
            }

            return View("PaymentStep", form);
        }

        public IActionResult Finish()
        {
            HttpContext.Session.RemoveCart();

            return View();
        }
    }
}

