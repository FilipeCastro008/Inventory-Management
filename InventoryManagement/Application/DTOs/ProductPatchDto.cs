namespace InventoryManagement.Application.DTOs {
    public class ProductPatchDto {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public int? AmountInStock { get; set; }
    }
}
