using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Entities;
using Booking.Data.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Booking.Core.ViewModels;
using AutoMapper;
using Booking.Web.Filters;
using Booking.Web.Extensions;
using Booking.Data.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Web.Controllers
{

    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly GymClassRepository gymClassRepository;
        private readonly ApplicationUserRepository applicationUserRepository;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            db = context;
            this.userManager = userManager;
            this.mapper = mapper;
            gymClassRepository = new GymClassRepository(context);
            applicationUserRepository = new ApplicationUserRepository(context);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(IndexViewModel viewModel = null)
        {
            var userId = userManager.GetUserId(User);
            var model = new IndexViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                model.GymClasses =  mapper.Map<IEnumerable<GymClassesViewModel>>
                                                     (await gymClassRepository.GetAsync());
               
            }

            if (viewModel.ShowHistory)
            {
                model.GymClasses = mapper.Map<IEnumerable<GymClassesViewModel>>(await gymClassRepository.GetHistory());
            }

            else
            {
                var gymclasses = await gymClassRepository.GetWithBookings();
                model.GymClasses = gymclasses
                                    .Select(g => new GymClassesViewModel
                                    {
                                        Id = g.Id,
                                        Name = g.Name,
                                        Duration = g.Duration,
                                        Attending = g.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                                    });
                                  
            }

            return View(model);
        }

      



        public async Task<IActionResult> GetBookings()
        {
            var userId = userManager.GetUserId(User);
           var gymClasses = await applicationUserRepository.GetBookings(userId);
            var model = new IndexViewModel
            {

                GymClasses = gymClasses.Select(g => new GymClassesViewModel
                {
                    Id = g.GymClass.Id,
                    Name = g.GymClass.Name,
                    Duration = g.GymClass.Duration,
                    Attending = g.GymClass.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                })
            };

            return View(nameof(Index), model);
        }

      

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id is null) return BadRequest();

            var userId = userManager.GetUserId(User);

            var attending = applicationUserRepository.GetAttending(id, userId);

            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                };

                applicationUserRepository.Add(booking);
                await db.SaveChangesAsync();
            }
            else
            {
               applicationUserRepository.Remove(attending);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }

       

        [RequiredIdRequiredModelFilter("id")]
        public async Task<IActionResult> Details(int? id)
        {
            GymClass gymClass = await gymClassRepository.GetAsync(id);

            return View(gymClass);
        }

      

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            if (Request.IsAjax())
                return PartialView("CreatePartial");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateGymClassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var gymClass = mapper.Map<GymClass>(viewModel);
                gymClassRepository.Add(gymClass);
                await db.SaveChangesAsync();

                if (Request.IsAjax())
                {
                    var model = new GymClassesViewModel
                    {
                        Id = gymClass.Id,
                        Name = gymClass.Name,
                        StartDate = gymClass.StartDate,
                        Duration = gymClass.Duration
                    };

                    return PartialView("GymClassPartial", model);
                }


                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [RequiredIdRequiredModelFilter("id")]
        public async Task<IActionResult> Edit(int? id)
        {
            var model = mapper.Map<EditGymClassViewModel>(await gymClassRepository.GetAsync(id));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EditGymClassViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var gymClass = mapper.Map<GymClass>(viewModel);
                try
                {
                    gymClassRepository.Update(gymClass);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [RequiredIdRequiredModelFilter("id")]
        public async Task<IActionResult> Delete(int? id)
        {
            var gymClass = await gymClassRepository.GetAsync(id);

            return View(gymClass);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await gymClassRepository.GetAsync(id);
            gymClassRepository.Remove(gymClass);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return gymClassRepository.Any(id);
        }
    }
}
