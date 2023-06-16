namespace Store.Data
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Isbn { get; set; }

        public string Author { get; set; }
        //название книги
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
