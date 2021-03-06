﻿using Booking.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Core.Repositories
{
    public interface IGymClassRepository
    {
        void Add(GymClass gymClass);
        bool Any(int id);
        Task<IEnumerable<GymClass>> GetAsync();
        Task<GymClass> GetAsync(int? id);
        Task<IEnumerable<GymClass>> GetHistoryAsync();
        Task<IEnumerable<GymClass>> GetWithBookingsAsync();
        void Remove(GymClass gymClass);
        void Update(GymClass gymClass);
    }
}