using System;

namespace ProductCatalog.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }


        // Associa uma Categoria a um Produto (one-to-one)
        public int CategoryId { get; set; } // Apenas Id da Categoria
        public Category Category { get; set; } // Todo o objeto

    }
}