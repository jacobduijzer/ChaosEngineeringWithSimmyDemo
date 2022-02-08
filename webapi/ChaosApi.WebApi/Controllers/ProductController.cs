using ChaosApi.WebApi.Products;
using Microsoft.AspNetCore.Mvc;

namespace ChaosApi.WebApi.Controllers;

public class ProductController : ControllerBase
{
   [HttpGet]
   [Route("/product/details/{productId}")]
   public async Task<IActionResult> ProductDetails([FromServices]IProductService productService, [FromServices]ILogger<ProductController> logger, int productId)
   {
      logger.LogInformation($"Requesting product details for product {productId}");
      var productDetails = await productService.ProductDetails(productId).ConfigureAwait(false);
      var upsales = await productService.UpsalesForProduct(productId).ConfigureAwait(false);
      return new OkObjectResult(new ProductDetails(productDetails, upsales));
   }
}