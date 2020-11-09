using AutoMapper;
using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Core.ViewModels;
using Booking.Data.Data;
using Booking.Tests.Extensions;
using Booking.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

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
