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
    public class VenuesController_Test
    {

        private readonly Mock<IVenueRepository> Repository;
        private readonly VenuesController Controller;

        public VenuesController_Test()
        {
            Repository = new Mock<IVenueRepository>();
            Controller = new VenuesController(Repository.Object);
        }

        [Fact]
        public  void Index_ShowCorrectView()
        {
            var result = Controller.Index(null);
           //implement
            Assert.Null(result);
        }

        [Fact]
        public void Index_ContainsCorrectModel()
        {
            var expectedResources = VenueGenerator.CreateList();
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
        public void Create_ContainsCorrectModel()
        {
            var expectedResources = VenueGenerator.CreateList();
            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public async void Delete_ShowsCorrectView()
        {
            Repository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(VenueGenerator.Create());
            var result = await Controller.Delete(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Delete_ContainsCorrectModel()
        {
            var expectedResource = VenueGenerator.Create();
            Repository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Delete(1) as ViewResult;
            Assert.IsType<Venue>(viewResult.Model);

            var resources = viewResult.Model as Venue;
            Assert.Equal(expectedResource, resources);
        }

        [Fact]
        public async void Details_ShowsCorrectView()
        {
            Repository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(VenueGenerator.Create());
            var result = await Controller.Delete(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Details_ContainsCorrectModel()
        {
            var expectedResource = VenueGenerator.Create();
            Repository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Details(1) as ViewResult;
            Assert.IsType<Venue>(viewResult.Model);

            var resources = viewResult.Model as Venue;
            Assert.Equal(expectedResource, resources);
        }

        [Fact]
        public async void DeleteConfirmed_DeletesVenue()
        {
            var venue = VenueGenerator.Create();

            var result = await Controller.DeleteConfirmed(venue.VenueId);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            Repository.Verify();
        }

        [Fact]
        public async void Update_ShowsCorrectView()
        {
            Repository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(VenueGenerator.Create());
            var result = await Controller.Edit(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Update_ContainsCorrectModel()
        {
            var expectedResource = VenueGenerator.Create();
            Repository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Edit(1) as ViewResult;
            Assert.IsType<Venue>(viewResult.Model);

            var resources = viewResult.Model as Venue;
            Assert.Equal(expectedResource, resources);
        }
    }
}
