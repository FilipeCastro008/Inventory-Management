using InventoryManagement.Data.Context;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data.Repositories {
    public class ProductRepository : IProductRepository {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id) {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product) {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product) {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var product = await GetByIdAsync(id);
            if (product != null) {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByNameAndDescriptionAsync(string name, string description) {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Name == name && p.Description == description);
        }
    }
}
