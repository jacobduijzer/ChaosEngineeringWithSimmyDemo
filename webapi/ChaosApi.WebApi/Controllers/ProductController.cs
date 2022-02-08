using ChaosApi.WebApi.Products;
using Microsoft.AspNetCore.Mvc;

namespace ChaosApi.WebApi.Controllers;

public class ProductController : ControllerBase
{
   private readonly IProductService _productService;
   private readonly ILogger<ProductController> _logger;

   public ProductController(IProductService productService, ILogger<ProductController> logger)
   {
      _productService = productService;
      _logger = logger;
   }

   [HttpGet]
   [Route("/product/details/{productId}")]
   public async Task<IActionResult> ProductDetails(int productId)
   {
      _logger.LogInformation($"Requesting product details for product {productId}");
      var productDetails = await _productService.ProductDetails(productId).ConfigureAwait(false);
      var upsales = await _productService.UpsalesForProduct(productId).ConfigureAwait(false);
      return new OkObjectResult(new ProductDetails(productDetails, upsales));
   }
}