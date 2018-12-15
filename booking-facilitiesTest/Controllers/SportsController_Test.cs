using AberFitnessAuditLogger;
using booking_facilities.Controllers;
using booking_facilities.Models;
using booking_facilities.Repositories;
using booking_facilitiesTest.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace booking_facilitiesTest.Controllers
{
    public class SportsController_Test
    {

        private readonly Mock<ISportRepository> Repository;
        private readonly SportsController Controller;
        private readonly IAuditLogger auditLogger;

        public SportsController_Test()
        {
            Repository = new Mock<ISportRepository>();
            Controller = new SportsController(Repository.Object, auditLogger);
        }

        //pagination on the index page makes this method difficult
        //also the use of IQueryables which is questionable design
        [Fact]
        public  void Index_ShowCorrectView()
        {
            var result = Controller.Index(null);
           //implement
            Assert.Null(result);
        }

        //pagination on the index page makes this method difficult
        //also the use of IQueryables which is questionable design
        [Fact]
        public async void Index_ContainsCorrectModel()
        {
            var expectedResources = SportGenerator.CreateList();
            //implement
            Assert.Equal(1,2);
        }

        [Fact]
        public void Create_ShowsCorrectView()
        {
            var result = Controller.Create();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Create_ContainsCorrectModel()
        {
            var expectedResource = SportGenerator.Create();
            var viewResult = Controller.Create() as ViewResult;
            Assert.IsType<Sport>(viewResult.Model);

            var resources = viewResult.Model as Sport;
            Assert.Equal(expectedResource, resources);
        }


        [Fact]
        public async void Delete_ShowsCorrectView()
        {
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SportGenerator.Create());
            var result = await Controller.Delete(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Delete_ContainsCorrectModel()
        {
            var expectedResource = SportGenerator.Create();
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Delete(1) as ViewResult;
            Assert.IsType<Sport>(viewResult.Model);

            var resources = viewResult.Model as Sport;
            Assert.Equal(expectedResource, resources);
        }

        [Fact]
        public async void Details_ShowsCorrectView()
        {
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SportGenerator.Create());
            var result = await Controller.Delete(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Details_ContainsCorrectModel()
        {
            var expectedResource = SportGenerator.Create();
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Details(1) as ViewResult;
            Assert.IsType<Sport>(viewResult.Model);

            var resources = viewResult.Model as Sport;
            Assert.Equal(expectedResource, resources);
        }

        [Fact]
        public async void DeleteConfirmed_DeletesApiResource()
        {
            var sport = SportGenerator.Create();

            var result = await Controller.DeleteConfirmed(sport.SportId);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            Repository.Verify();
        }

        [Fact]
        public async void Update_ShowsCorrectView()
        {
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SportGenerator.Create());
            var result = await Controller.Edit(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Update_ContainsCorrectModel()
        {
            var expectedResource = SportGenerator.Create();
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Edit(1) as ViewResult;
            Assert.IsType<Sport>(viewResult.Model);

            var resources = viewResult.Model as Sport;
            Assert.Equal(expectedResource, resources);
        }
    }
}
