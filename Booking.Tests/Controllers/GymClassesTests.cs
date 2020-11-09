using AutoMapper;
using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Core.ViewModels;
using Booking.Data.Data;
using Booking.Tests.Extensions;
using Booking.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Tests.Controllers
{
    [TestClass]
    public class GymClassesTests
    {

        private Mock<IGymClassRepository> repository;
        private IMapper mapper;
        private GymClassesController controller;

       [TestInitialize]
       public void SetUp()
        {
            repository = new Mock<IGymClassRepository>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.GymClassRepository).Returns(repository.Object);

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                var profile = new MapperProfile();
                cfg.AddProfile(profile);
            }));

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            controller = new GymClassesController(mockUoW.Object, mockUserManager.Object, mapper);
        }

        [TestMethod]
        public void Index_NotAutenthicated_ReturnsExpected()
        {
            var gymclasses = GetGymClassList();
            var expected = new IndexViewModel { GymClasses = mapper.Map<IEnumerable<GymClassesViewModel>>(gymclasses) };

            controller.SetUserIsAuthenticated(false);
            repository.Setup(r => r.GetAsync()).ReturnsAsync(gymclasses);
            var vm = new IndexViewModel { ShowHistory = false };

            var viewResult = controller.Index(vm).Result as ViewResult;

            var actual = (IndexViewModel)viewResult.Model;

            Assert.AreEqual(expected.GymClasses.Count(), actual.GymClasses.Count());

        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_ShouldPass()
        {
            controller.SetUserIsAuthenticated(true);
            var vm = new IndexViewModel { ShowHistory = false };

            var actual = await controller.Index(vm);

            Assert.IsInstanceOfType(actual, typeof(ViewResult));
        }

        [TestMethod]
        public void Crete_ReturnsDefaultView_ShouldReturnNull()
        {
            controller.SetAjaxRequest(false);
            var result = controller.Create() as ViewResult;

            Assert.IsNull(result.ViewName);
        } 
        
        [TestMethod]
        public void Crete_ReturnsCreatePartialWhenAjax_ShouldReturnCreatePartial()
        {
            const string viewName = "CreatePartial";
            controller.SetAjaxRequest(true);
            var result = controller.Create() as PartialViewResult;

            Assert.AreEqual(result.ViewName, viewName);
        }




        private List<GymClass> GetGymClassList()
        {
            return new List<GymClass>
            {
                new GymClass
                {
                    Id = 1,
                    Name = "Spinning",
                    Description = "Hard",
                    StartDate = DateTime.Now.AddDays(3),
                    Duration = new TimeSpan(0,60,0)
                },
                new GymClass
                {
                    Id = 2,
                    Name = "HyperFys",
                    Description = "Harder",
                    StartDate = DateTime.Now.AddDays(-3),
                    Duration = new TimeSpan(0,60,0)
                }
            };
        }

    }
}
