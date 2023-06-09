using e_commerce_store.Models;
using e_commerce_store.Models.Interfaces;
using e_commerce_store.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_store.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class DashboardController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICategoryRepository _categoryRepository;

        public DashboardController(IProductRepository productRepository , IOrderRepository orderRepository , ICategoryRepository categoryRepository){
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _categoryRepository = categoryRepository;
        }


        public IActionResult Index(){
            return RedirectToAction("Products");
        }

        public async Task<IActionResult> Products(string? searchString, int categoryId = -1, int page = 1, int pageSize = 10){
            if (page < 1 || pageSize < 1 || categoryId < -1 || pageSize > 40)
            {
                return NotFound();
            }

            IEnumerable<Product> products;
            int count;

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals(""))
            {
                if(categoryId == -1){
                    products = await _productRepository.SearchAndSliceAsync(searchString,(page - 1) * pageSize, pageSize);
                    count = await _productRepository.GetCountBySearchAsync(searchString);
                }else{
                    products = await _productRepository.SearchByCategoryAndSliceAsync(searchString,categoryId,(page - 1) * pageSize, pageSize);
                    count = await _productRepository.GetCountBySearchWithCategoryAsync(searchString,categoryId);
                }
            }else
            {
                count = categoryId switch
                {
                    -1 => await _productRepository.GetCountAsync(),
                    _ => await _productRepository.GetCountByCategoryAsync(categoryId),
                };
                products = categoryId switch
                {
                    -1 => await _productRepository.GetSliceAsync((page - 1) * pageSize, pageSize),
                    _ => await _productRepository.GetProductsByCategoryAndSliceAsync(categoryId, (page - 1) * pageSize, pageSize),
                };
                
            }

            var productViewModel = new IndexProductViewModel
            {
                Products = products,
                Page = page,
                PageSize = pageSize,
                TotalProducts = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                CategoryId = categoryId,
                Categories = _categoryRepository.GetAll(),
                searchString = searchString
            };

            return View(productViewModel);    
        }//End of product view

        public async Task<IActionResult> Orders(string? searchString , int page = 1, int pageSize = 10){
            if (page < 1 || pageSize < 1  || pageSize > 40)
            {
                return NotFound();
            }
            IEnumerable<Order> orders;
            int count;

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("")){
                orders = await _orderRepository.SearchAndSliceAsync(searchString,(page - 1) * pageSize, pageSize);
                count = await _orderRepository.GetCountBySearchAsync(searchString);
            }else{
                orders = await _orderRepository.GetSliceAsync((page - 1) * pageSize, pageSize);
                count = await _orderRepository.GetCountAsync();
            }

            var OrderViewModel = new IndexOrderViewModel
            {
                Orders = orders,
                Page = page,
                PageSize = pageSize,
                TotalProducts = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                searchString = searchString
            };

            return View(OrderViewModel);         
        }//End of order view model
        
        public async Task<IActionResult> Categories(string? searchString , int page = 1, int pageSize = 10){
            if (page < 1 || pageSize < 1  || pageSize > 40)
            {
                return NotFound();
            }
            IEnumerable<Category> categories;
            int count;

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("")){
                categories = await _categoryRepository.SearchAndSliceAsync(searchString,(page - 1) * pageSize, pageSize);
                count = await _categoryRepository.GetCountBySearchAsync(searchString);
            }else{
                categories = await _categoryRepository.GetSliceAsync((page - 1) * pageSize, pageSize);
                count = await _categoryRepository.GetCountAsync();
            }

            var CategoryViewModel = new IndexCategoryViewModel
            {
                Categories = categories,
                Page = page,
                PageSize = pageSize,
                TotalProducts = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                searchString = searchString
            };

            return View(CategoryViewModel);         
        }//End of order view model
        

        
    }
}