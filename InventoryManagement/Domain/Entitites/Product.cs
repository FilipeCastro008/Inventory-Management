using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities {
    public class Product {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
        
        [Display(Name = "Amount in stock")]
        public int AmountInStock { get; set; }
    }
}

