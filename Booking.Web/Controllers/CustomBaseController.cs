using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Booking.Core.Entities;
using Booking.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Web.Controllers
{
    public class CustomBaseController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; }
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public CustomBaseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            UserManager = userManager;
            Mapper = mapper;
            UnitOfWork = unitOfWork;
        }

    }
}
