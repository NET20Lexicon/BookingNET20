using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repositories
{
    public class GymClassRepository : IGymClassRepository
    {
        private readonly ApplicationDbContext db;

        public GymClassRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<GymClass>> GetAsync()
        {
            return await db.GymClasses.ToListAsync();
        }

        public async Task<GymClass> GetAsync(int? id)
        {
            return await db.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<GymClass>> GetHistoryAsync()
        {
            return await db.GymClasses
                        .IgnoreQueryFilters()
                        .Where(g => g.StartDate < DateTime.Now).ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetWithBookingsAsync()
        {
            return await db.GymClasses.Include(g => g.AttendedMembers).ToListAsync();
        }

        public void Add(GymClass gymClass)
        {
            db.Add(gymClass);
        }
        public void Update(GymClass gymClass)
        {
            db.Update(gymClass);
        }

        public void Remove(GymClass gymClass)
        {
            db.Remove(gymClass);
        }

        public bool Any(int id)
        {
            return db.GymClasses.Any(g => g.Id == id);
        }
    }
}
