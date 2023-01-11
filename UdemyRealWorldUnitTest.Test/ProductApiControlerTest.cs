using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.WEB.Controllers;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductApiControlerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private List<Product> products;
        public ProductApiControlerTest()
        {
            _mockRepo= new Mock<IRepository<Product>>();
            _controller= new ProductsApiController(_mockRepo.Object);

            products = new List<Product> { new Product
            {Id=1,Name="Kalem",Price=100,Stock=50,Color="Kırmızı" },
             {
              new Product {Id=2,Name="Defter",Price=120,Stock=500,Color="Mavi"} } };

        }
        //[Fact]
        //public async void GetProduct_ActionExecuteOkResultWhitProduct()
        //{
        //    _mockRepo.Setup(x=>x.GetAll()).ReturnsAsync(products);
        //   var result =  await _controller.GetProduct();
        //    var okresult = Assert.IsType<OkObjectResult>(result);
        //    var returnProduct = Assert.IsAssignableFrom<List<IEnumerable<Product>>>(okresult.Value);
        //    Assert.Equal<int>(2, returnProduct.ToList().Count);
        //}
        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product= null;
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);
            Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = products.First(x => x.Id == productId);
            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);
            var result= await _controller.GetProduct(productId);
            var okresult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<Product>(okresult.Value);
            Assert.Equal(productId,returnProduct.Id);
            Assert.Equal(product.Name,returnProduct.Name);
        }
        [Theory]
        [InlineData(1)]
        
        public void PutProduct_IdIsNotEqualProduct_BadRequest(int ProductId)
        {
            var Product = products.First(x=>x.Id == ProductId);
            var result = _controller.PutProduct(2, Product);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int ProductId)
        {
            var product = products.First(x=>x.Id==ProductId);
            _mockRepo.Setup(x => x.Update(product));
            var result =_controller.PutProduct(ProductId, product);
            _mockRepo.Verify(x => x.Update(product), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
       
        [Fact]
        public async void PostProduct_ActionExecute_ReturnCreatedAtAction()
        {
            var product = products.First();
            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);
            var result = await _controller.PostProduct(product);
            var CreatedAtActionResult = 
                Assert.IsType<CreatedAtActionResult>(result);
            _mockRepo.Verify(x => x.Create(product), Times.Once);
            Assert.Equal("GetProduct", CreatedAtActionResult.ActionName);
        }



    }
}
