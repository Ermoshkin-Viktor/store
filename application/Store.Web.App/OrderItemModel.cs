namespace Store.Web.Models
{
    //Позиция заказа
    //DTO( Data Transfer Object- специальная модель для передачи данных.)
    public class OrderItemModel
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }
    }
}
