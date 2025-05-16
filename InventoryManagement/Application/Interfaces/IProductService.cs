using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces {
    public interface IProductService {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<(bool success, string errorMessage)> UpdateFromDtoAsync(int id, ProductPatchDto dto);
        Task<(bool Success, string? ErrorMessage, Product? Result)> AddAsync(ProductPatchDto dto);
        Task<bool> DeleteAsync(int id);
        Task UpdateAsync(Product product);
        Task<(bool Success, string? ErrorMessage)> RemoveUnitAsync(int productId, int quantity);
        Task<(bool Success, string? ErrorMessage)> AddUnitAsync(int productId, int quantity);
    }
}
