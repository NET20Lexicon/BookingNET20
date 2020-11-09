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
using Booking.Core.Repositories;

namespace Booking.Web.Controllers
{

    public class GymClassesController : CustomBaseController
    {
        
        public GymClassesController(IUnitOfWork UnitOfWork, UserManager<ApplicationUser> UserManager, IMapper Mapper) : base(UnitOfWork, UserManager, Mapper)
        {
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(IndexViewModel viewModel = null)
        {
            if (viewModel is null) return BadRequest();

            var userId = UserManager.GetUserId(User);
            var model = new IndexViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                model.GymClasses =  Mapper.Map<IEnumerable<GymClassesViewModel>>
                                                     (await UnitOfWork.GymClassRepository.GetAsync());
               
            }

            if (viewModel.ShowHistory)
            {
                model.GymClasses = Mapper.Map<IEnumerable<GymClassesViewModel>>(await UnitOfWork.GymClassRepository.GetHistoryAsync());
            }

            if(User.Identity.IsAuthenticated && !viewModel.ShowHistory)
            {
                //var gymClasses = await UnitOfWork.GymClassRepository.GetWithBookingsAsync();
                //model.GymClasses = Mapper.Map<IEnumerable<GymClassesViewModel>>(gymClasses, opt => opt.Items.Add("Id", userId));
                //model = Mapper.Map<IndexViewModel>(gymClasses, opt => opt.Items.Add("Id", userId));



                //Old
                var gymclasses = await UnitOfWork.GymClassRepository.GetWithBookingsAsync();
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

        

        public IActionResult Fetch()
        {
            return PartialView("CreatePartial");
        }


        public async Task<IActionResult> GetBookings()
        {
            var userId = UserManager.GetUserId(User);
           var gymClasses = await UnitOfWork.AppUserRepository.GetBookingsAsync(userId);
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

            var userId = UserManager.GetUserId(User);

            var attending = UnitOfWork.AppUserRepository.GetAttending(id, userId);

            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                };

                UnitOfWork.AppUserRepository.Add(booking);
                await UnitOfWork.CompleteAsync();
            }
            else
            {
                UnitOfWork.AppUserRepository.Remove(attending);
                await UnitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));

        }

       

        [RequiredIdRequiredModelFilter("id")]
        public async Task<IActionResult> Details(int? id)
        {
            GymClass gymClass = await UnitOfWork.GymClassRepository.GetAsync(id);

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
                var gymClass = Mapper.Map<GymClass>(viewModel);
                UnitOfWork.GymClassRepository.Add(gymClass);
                await UnitOfWork.CompleteAsync();

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
            var model = Mapper.Map<EditGymClassViewModel>(await UnitOfWork.GymClassRepository.GetAsync(id));

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
                var gymClass = Mapper.Map<GymClass>(viewModel);
                try
                {
                    UnitOfWork.GymClassRepository.Update(gymClass);
                    await UnitOfWork.CompleteAsync();
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
            var gymClass = await UnitOfWork.GymClassRepository.GetAsync(id);

            return View(gymClass);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await UnitOfWork.GymClassRepository.GetAsync(id);
            UnitOfWork.GymClassRepository.Remove(gymClass);
            await UnitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return UnitOfWork.GymClassRepository.Any(id);
        }
    }
}
