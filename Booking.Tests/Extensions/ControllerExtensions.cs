using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Tests.Extensions
{
    public static class ControllerExtensions
    {
        public static void SetUserIsAuthenticated(this Controller controller,  bool isAuthenticated)
        {
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(context => context.User.Identity.IsAuthenticated).Returns(isAuthenticated);
            controller.ControllerContext = new ControllerContext { HttpContext = mockContext.Object };
        }

        public static void SetAjaxRequest(this Controller controller, bool isAjax)
        {
            var mockContext = new Mock<HttpContext>();
            if (isAjax)
                mockContext.SetupGet(c => c.Request.Headers["X-Requested-With"]).Returns("XMLHttpRequest");
            else
                mockContext.SetupGet(c => c.Request.Headers["X-Requested-With"]).Returns("");

            controller.ControllerContext = new ControllerContext { HttpContext = mockContext.Object };
        }
    }
}
