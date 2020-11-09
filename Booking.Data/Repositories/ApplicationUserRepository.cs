using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Data.Data;
using Booking.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext db;

        public ApplicationUserRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<ApplicationUserGymClass>> GetBookingsAsync(string userId)
        {
            return await db.ApplicationUserGymClasses
                                           .Include(g => g.GymClass)
                                           .IgnoreQueryFilters()
                                           .Where(u => u.ApplicationUserId == userId)
                                           .ToListAsync();

        }

        public ApplicationUserGymClass GetAttending(int? id, string userId)
        {
            return db.ApplicationUserGymClasses.Find(userId, id);
        }

        public void Add(ApplicationUserGymClass attending)
        {
            db.Add(attending);
        }

        public void Remove(ApplicationUserGymClass attending)
        {
            db.Remove(attending);
        }

        public async Task<IEnumerable<string>> GetBookedMemberNamesAsync(int gymClassId)
        {

            return await db.ApplicationUserGymClasses
                                     .Include(g => g.ApplicationUser)
                                     .Where(g => g.GymClassId == gymClassId)
                                     .Select(a => a.ApplicationUser.FullName)
                                     .ToListAsync();
        }
    }
}
