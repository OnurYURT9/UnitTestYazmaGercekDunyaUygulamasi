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
            _mockrepo = new Mock<IRepository<Product>>();
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
        [Fact]
        public async void Details_Id_IsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async void Details_IdInvalid_ReturnNotFound()
        {
            Product product = null;
            _mockrepo.Setup(repo => repo.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);
            _mockrepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Details(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        [Fact]
        public void Create_Action_Execute_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Create_POST_InvalidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name",
                "Name alanı belirtilmedi");
            var result = await _controller.Create(products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }
        [Fact]
        public async void Create_POST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecuted()
        {
            Product newProduct = null;
            _mockrepo.Setup(repo => repo.Create(It.IsAny<Product>
                ())).Callback<Product>(x => newProduct = x);
            var result = await _controller.Create(products.First());
            _mockrepo.Verify(repo => repo.Create(It.IsAny<Product>()),
                Times.Once());
            Assert.Equal(products.First().Id, newProduct.Id);
        }
        [Fact]
        public async void CreatePOST_InValidModelState_NeverCreate()
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = await _controller.Create(products.First());
            _mockrepo.Verify(repo => repo.Create(It.IsAny<Product>()),
                Times.Never());
        }

        [Fact]
        public async void EditIsNull_ReturnToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(3)]
        public async void EditIdIsValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockrepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(2)]
        public async void Edit_Action_Execute_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);
            _mockrepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOSTId_IsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, products.First(x => x.Id == productId));
            var redirect = Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public void EditPOST_IsValidModelState_ReturnsView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = _controller.Edit(productId, products.First(x => x.Id == productId));
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);

        }
        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _controller.Edit(productId,products.First(x=>x.Id ==
            productId));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int ProductId)
        {
            var product = products.First(x => x.Id == ProductId);
            _mockrepo.Setup(repo => repo.Update(product));
            _controller.Edit(ProductId, product);
            _mockrepo.Verify(repo => repo.Update(It.IsAny<Product>()),Times.Once);
        }
        [Fact]
        public async void Delete_IdNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
           Assert.IsType<NotFoundResult>(result);
        }
        //Olmayan veriyi test ettik
        [Theory]
        [InlineData(0)]
        public async void Delete_Id_IsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockrepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);

        }
        //var olan veriyi test ettik 
        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x=>x.Id== productId);
            _mockrepo.Setup(repo=>repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecute_ReturnRedirectToAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = products.First(x=>x.Id == productId);
            _mockrepo.Setup(repo => repo.Delete(product));
           await _controller.DeleteConfirmed(productId);
            _mockrepo.Verify(repo=>repo.Delete(It.IsAny<Product>()),Times.Once);
        }
    }

}
