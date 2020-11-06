using Booking.Core.Entities;
using Booking.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repositories
{
    public class GymClassRepository
    {
        private readonly ApplicationDbContext db;

        public GymClassRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<GymClass>>  GetAsync()
        {
            return await db.GymClasses.ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetHistory()
        {
            return await db.GymClasses
                        .IgnoreQueryFilters()
                        .Where(g => g.StartDate < DateTime.Now).ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetWithBookings()
        {
            return await db.GymClasses.Include(g => g.AttendedMembers).ToListAsync();
        }
    }
}
