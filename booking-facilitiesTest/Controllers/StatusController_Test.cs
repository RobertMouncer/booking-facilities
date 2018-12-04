using booking_facilities.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace booking_facilitiesTest.Controllers
{
    public class StatusController_Test
    {
        [Fact]
        public void Get_ReturnsOk()
        {
            var controller = new StatusAPIController();
            var result = controller.Get();
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Get_HasCorrectContent()
        {
            var controller = new StatusAPIController();
            var result = controller.Get();
            var content = result as OkObjectResult;
            Assert.Null(content);
        }
    }
}
