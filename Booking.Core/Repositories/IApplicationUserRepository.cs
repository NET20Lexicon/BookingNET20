using Booking.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Core.Repositories
{
    public interface IApplicationUserRepository
    {
        void Add(ApplicationUserGymClass attending);
        ApplicationUserGymClass GetAttending(int? id, string userId);
        Task<IEnumerable<ApplicationUserGymClass>> GetBookings(string userId);
        void Remove(ApplicationUserGymClass attending);
    }
}