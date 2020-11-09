using AutoMapper;
using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Web.Controllers;
using Booking.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Tests.Filters
{
    [TestClass]
    public class RequiredIdRequiredModelFilterTests
    {
        private Mock<IGymClassRepository> repository;
        private GymClassesController controller;

        [TestInitialize]
        public void SetUp()
        {
            repository = new Mock<IGymClassRepository>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.GymClassRepository).Returns(repository.Object);

            var mapper = new Mock<IMapper>();

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUsermanager = new Mock<UserManager<ApplicationUser>>
                (userStore.Object, null, null, null, null, null, null, null, null);

            controller = new GymClassesController(mockUoW.Object, mockUsermanager.Object, mapper.Object);
        }

        [TestMethod]
        public void Details_NullId_ShouldReturnNotFound()
        {
            var routeValues = new RouteValueDictionary();
            routeValues.Add("id", null);
            var routeData = new RouteData(routeValues);

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                routeData,
                Mock.Of<ActionDescriptor>(),
                Mock.Of<ModelStateDictionary>()
                );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                routeValues,
                controller
                );

            var filter = new RequiredIdRequiredModelFilter("Id");
            filter.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
