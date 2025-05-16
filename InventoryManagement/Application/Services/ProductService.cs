using InventoryManagement.Application.DTOs;
using System.Globalization;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Services {
    public class ProductService : IProductService {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger) {
            _repository = repository;
            _logger = logger;
        }

        #region Services Methods
        public async Task<IEnumerable<Product>> GetAllAsync() {
            try {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erro ao buscar todos os produtos.");
                return Enumerable.Empty<Product>();
            }
        }

        public async Task<Product?> GetByIdAsync(int id) {
            try {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao buscar produto pelo ID {id}.");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage, Product? Result)> AddAsync(ProductPatchDto dto) {
            try {
                var product = new Product();

                if (!TryApplyPatchToProduct(product, dto, out var errorMessage))
                    return (false, errorMessage, product);

                var existingProduct = await _repository.GetByNameAndDescriptionAsync(product.Name, product.Description);
                if (existingProduct != null)
                    return (false, "Já existe um produto com o mesmo nome e descrição.", product);

                if (product.AmountInStock <= 0)
                    product.AmountInStock = 1;

                await _repository.AddAsync(product);
                return (true, null, product);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erro ao adicionar produto.");
                return (false, $"Erro ao adicionar produto: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string errorMessage)> UpdateFromDtoAsync(int id, ProductPatchDto dto) {
            try {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return (false, "Produto não encontrado.");

                if (!TryApplyPatchToProduct(product, dto, out var errorMessage))
                    return (false, errorMessage);

                await _repository.UpdateAsync(product);
                return (true, null);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao atualizar produto ID {id}.");
                return (false, $"Erro ao atualizar produto: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Product product) {
            try {
                await _repository.UpdateAsync(product);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao atualizar produto ID {product.Id}.");
                throw; // relança a exceção para a camada superior decidir o que fazer
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> AddUnitAsync(int productId, int quantity) {
            try {
                if (quantity <= 0)
                    return (false, "A quantidade deve ser maior que zero.");

                var product = await _repository.GetByIdAsync(productId);
                if (product == null)
                    return (false, "Produto não encontrado.");

                product.AmountInStock += quantity;
                await _repository.UpdateAsync(product);

                return (true, null);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao adicionar unidade ao produto ID {productId}.");
                return (false, $"Erro ao adicionar unidade: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveUnitAsync(int productId, int quantity) {
            try {
                if (quantity <= 0)
                    return (false, "A quantidade deve ser maior que zero.");

                var product = await _repository.GetByIdAsync(productId);
                if (product == null)
                    return (false, "Produto não encontrado.");

                if (product.AmountInStock < quantity)
                    return (false, "A quantidade solicitada excede o estoque atual.");

                product.AmountInStock -= quantity;
                await _repository.UpdateAsync(product);

                return (true, null);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao remover unidade do produto ID {productId}.");
                return (false, $"Erro ao remover unidade: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(int id) {
            try {
                var product = await _repository.GetByIdAsync(id);

                if (product == null)
                    return false;

                if (product.AmountInStock > 0)
                    return false;

                await _repository.DeleteAsync(product.Id);
                return true;
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao deletar produto ID {id}.");
                return false;
            }
        }
        #endregion

        #region Private Methods
        private bool TryApplyPatchToProduct(Product product, ProductPatchDto dto, out string? errorMessage) {
            foreach (var prop in typeof(ProductPatchDto).GetProperties()) {
                var newValue = prop.GetValue(dto);
                if (newValue == null) continue;

                var targetProp = typeof(Product).GetProperty(prop.Name);
                if (targetProp == null || !targetProp.CanWrite) continue;

                if (prop.Name == "Price" && newValue is string priceStr) {
                    priceStr = priceStr.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim();

                    if (!decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice)) {
                        errorMessage = "Preço inválido.";
                        return false;
                    }

                    if (parsedPrice <= 0) {
                        errorMessage = "O preço deve ser maior que zero.";
                        return false;
                    }

                    targetProp.SetValue(product, parsedPrice);
                }
                else {
                    targetProp.SetValue(product, newValue);
                }
            }

            errorMessage = null;
            return true;
        }
        #endregion
    }
}
