using System.Collections.Generic;

namespace ProductCatalog.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Title { get; set; }
        
        // Associa vários produtos a uma categoria (One-to-Many)
        // Enumerador / Lista / Coleção de Produtos
        public IEnumerable<Product> Products { get; set; }
        
    }
}
