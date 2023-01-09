using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UdemyRealWorldUnitTest.WEB.Controllers;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockrepo;

        private readonly ProductsController _controller;
        private List<Product> products; 
        public ProductControllerTest()
        {
            _mockrepo= new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockrepo.Object);
            products = new List<Product> { new Product
            {Id=1,Name="Kalem",Price=100,Stock=50,Color="Kırmızı" },
             {
              new Product {Id=2,Name="Defter",Price=120,Stock=500,Color="Mavi"} } };
        }
        [Fact]
        public async void Index_ActionExecute_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Index_ActionExecuteReturnProductList()
        {
            _mockrepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);
            var result = await _controller.Index(); 
            var ViewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>
                (ViewResult.Model);
            Assert.Equal<int>(2, productList.Count());
        }
    }
}
