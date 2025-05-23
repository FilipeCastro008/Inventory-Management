﻿using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Domain.Interfaces {
    public interface IProductRepository {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Product?> GetByNameAndDescriptionAsync(string name, string description);
    }
}
