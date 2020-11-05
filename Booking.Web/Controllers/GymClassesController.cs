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

namespace Booking.Web.Controllers
{

    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            db = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            var model = new IndexViewModel
            {
                GymClasses = await db.GymClasses.Include(g => g.AttendedMembers)
                                    .Select(g => new GymClassesViewModel
                                    {
                                        Id = g.Id,
                                        Name = g.Name,
                                        Duration = g.Duration,
                                        Attending = g.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                                    })
                                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> GetBookings()
        {
            var userId = userManager.GetUserId(User);
            var model = new IndexViewModel
            {
                GymClasses = await db.ApplicationUserGymClasses
                                    .IgnoreQueryFilters()
                                    .Where(u => u.ApplicationUserId == userId)
                                    .Select(g => new GymClassesViewModel
                                    {
                                        Id = g.GymClass.Id,
                                        Name = g.GymClass.Name,
                                        Duration = g.GymClass.Duration,
                                        Attending = g.GymClass.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                                    })
                                    .ToListAsync()
            };

            return View(nameof(Index), model);
    }




    public async Task<IActionResult> BookingToggle(int? id)
    {
        if (id is null) return BadRequest();

        var userId = userManager.GetUserId(User);

        var attending = db.ApplicationUserGymClasses.Find(userId, id);

        if (attending is null)
        {
            var booking = new ApplicationUserGymClass
            {
                ApplicationUserId = userId,
                GymClassId = (int)id
            };

            db.ApplicationUserGymClasses.Add(booking);
            await db.SaveChangesAsync();
        }
        else
        {
            db.ApplicationUserGymClasses.Remove(attending);
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));

    }

    [RequiredIdRequiredModelFilter("id")]
    public async Task<IActionResult> Details(int? id)
    {
        var gymClass = await db.GymClasses
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return View(gymClass);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
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
            db.Add(gymClass);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [RequiredIdRequiredModelFilter("id")]
    public async Task<IActionResult> Edit(int? id)
    {
        var model = mapper.Map<EditGymClassViewModel>(await db.GymClasses.FindAsync(id));

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
                db.Update(gymClass);
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
        var gymClass = await db.GymClasses
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return View(gymClass);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var gymClass = await db.GymClasses.FindAsync(id);
        db.GymClasses.Remove(gymClass);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GymClassExists(int id)
    {
        return db.GymClasses.Any(e => e.Id == id);
    }
}
}
