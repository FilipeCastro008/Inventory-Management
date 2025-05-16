using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ProductServiceTests {
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _service;

    public ProductServiceTests() {
        _repositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts_WhenRepositoryReturnsData() {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Prod 1" },
            new Product { Id = 2, Name = "Prod 2" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenRepositoryThrows() {
        _repositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Erro"));

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenFound() {
        var product = new Product { Id = 1, Name = "Produto" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound() {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnFalse_WhenProductExists() {
        var dto = new ProductPatchDto { Name = "Test", Description = "Desc", Price = "R$10,00" };

        _repositoryMock.Setup(r => r.GetByNameAndDescriptionAsync("Test", "Desc"))
            .ReturnsAsync(new Product { Id = 99, Name = "Test", Description = "Desc" });

        var result = await _service.AddAsync(dto);

        Assert.False(result.Success);
        Assert.Equal("Já existe um produto com o mesmo nome e descrição.", result.ErrorMessage);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenProductAdded() {
        var dto = new ProductPatchDto { Name = "Test", Description = "Desc", Price = "R$10,00", AmountInStock = 5 };

        _repositoryMock.Setup(r => r.GetByNameAndDescriptionAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Product?)null);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await _service.AddAsync(dto);

        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);
        Assert.Equal("Test", result.Result?.Name);
        Assert.Equal(5, result.Result?.AmountInStock);
    }

    [Fact]
    public async Task UpdateFromDtoAsync_ShouldReturnFalse_WhenProductNotFound() {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var dto = new ProductPatchDto { Name = "Test" };

        var result = await _service.UpdateFromDtoAsync(1, dto);

        Assert.False(result.success);
        Assert.Equal("Produto não encontrado.", result.errorMessage);
    }

    [Fact]
    public async Task UpdateFromDtoAsync_ShouldReturnTrue_WhenUpdated() {
        var product = new Product { Id = 1, Name = "OldName", Description = "OldDesc" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

        var dto = new ProductPatchDto { Name = "NewName" };

        var result = await _service.UpdateFromDtoAsync(1, dto);

        Assert.True(result.success);
        Assert.Null(result.errorMessage);
        Assert.Equal("NewName", product.Name);
    }

    [Fact]
    public async Task AddUnitAsync_ShouldReturnFalse_WhenQuantityInvalid() {
        var result = await _service.AddUnitAsync(1, 0);

        Assert.False(result.Success);
        Assert.Equal("A quantidade deve ser maior que zero.", result.ErrorMessage);
    }

    [Fact]
    public async Task AddUnitAsync_ShouldReturnFalse_WhenProductNotFound() {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.AddUnitAsync(1, 5);

        Assert.False(result.Success);
        Assert.Equal("Produto não encontrado.", result.ErrorMessage);
    }

    [Fact]
    public async Task AddUnitAsync_ShouldReturnTrue_WhenSuccessful() {
        var product = new Product { Id = 1, AmountInStock = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

        var result = await _service.AddUnitAsync(1, 5);

        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(15, product.AmountInStock);
    }

    [Fact]
    public async Task RemoveUnitAsync_ShouldReturnFalse_WhenQuantityInvalid() {
        var result = await _service.RemoveUnitAsync(1, 0);

        Assert.False(result.Success);
        Assert.Equal("A quantidade deve ser maior que zero.", result.ErrorMessage);
    }

    [Fact]
    public async Task RemoveUnitAsync_ShouldReturnFalse_WhenProductNotFound() {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.RemoveUnitAsync(1, 5);

        Assert.False(result.Success);
        Assert.Equal("Produto não encontrado.", result.ErrorMessage);
    }

    [Fact]
    public async Task RemoveUnitAsync_ShouldReturnFalse_WhenQuantityExceedsStock() {
        var product = new Product { Id = 1, AmountInStock = 3 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.RemoveUnitAsync(1, 5);

        Assert.False(result.Success);
        Assert.Equal("A quantidade solicitada excede o estoque atual.", result.ErrorMessage);
    }

    [Fact]
    public async Task RemoveUnitAsync_ShouldReturnTrue_WhenSuccessful() {
        var product = new Product { Id = 1, AmountInStock = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

        var result = await _service.RemoveUnitAsync(1, 5);

        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(5, product.AmountInStock);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound() {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenAmountInStockGreaterThanZero() {
        var product = new Product { Id = 1, AmountInStock = 5 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted() {
        var product = new Product { Id = 1, AmountInStock = 0 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.DeleteAsync(product.Id)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
