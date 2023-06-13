namespace Store.Data
{
    //позиция заказа
    public class OrderItemDto
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
        //Навигационное свойство
        public OrderDto Order { get; set; }
    }
}
