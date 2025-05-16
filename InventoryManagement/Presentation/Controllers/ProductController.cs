using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Presentation.Controllers {
    public class ProductController : Controller {
        private readonly IProductService _service;

        public ProductController(IProductService service) {
            _service = service;
        }

        public async Task<IActionResult> Index() {
            try {
                var products = await _service.GetAllAsync();
                return View(products);
            }
            catch (Exception ex) {
                TempData["ErrorMessage"] = "Erro ao carregar os produtos.";
                // Logar o erro se tiver logger
                return View(new List<Product>());
            }
        }

        public async Task<IActionResult> Details(int id) {
            try {
                var product = await _service.GetByIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch {
                TempData["ErrorMessage"] = "Erro ao carregar os detalhes do produto.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create() {
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductPatchDto dto) {
            try {
                var (success, errorMessage, product) = await _service.AddAsync(dto);

                if (!success) {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(product);
                }

                return RedirectToAction(nameof(Index));
            }
            catch {
                ModelState.AddModelError(string.Empty, "Erro ao criar o produto.");
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id) {
            try {
                var product = await _service.GetByIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch {
                TempData["ErrorMessage"] = "Erro ao carregar o produto para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(int id, ProductPatchDto dto) {
            try {
                var (success, errorMessage) = await _service.UpdateFromDtoAsync(id, dto);

                if (!success) {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View("Edit", await _service.GetByIdAsync(id));
                }

                return RedirectToAction(nameof(Index));
            }
            catch {
                ModelState.AddModelError(string.Empty, "Erro ao editar o produto.");
                return View("Edit", await _service.GetByIdAsync(id));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUnit(int id, int quantity) {
            try {
                var (success, errorMessage) = await _service.RemoveUnitAsync(id, quantity);
                if (!success)
                    TempData["ErrorMessage"] = errorMessage;
            }
            catch {
                TempData["ErrorMessage"] = "Erro ao remover unidades do produto.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUnit(int id, int quantity) {
            try {
                var (success, errorMessage) = await _service.AddUnitAsync(id, quantity);
                if (!success)
                    TempData["ErrorMessage"] = errorMessage;
            }
            catch {
                TempData["ErrorMessage"] = "Erro ao adicionar unidades ao produto.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id) {
            try {
                var product = await _service.GetByIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch {
                TempData["ErrorMessage"] = "Erro ao carregar o produto para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            try {
                var success = await _service.DeleteAsync(id);
                if (!success) {
                    TempData["ErrorMessage"] = "O produto não pode ser removido pois ainda possui unidades em estoque.";
                }
            }
            catch {
                TempData["ErrorMessage"] = "Erro ao excluir o produto.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
