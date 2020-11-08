﻿using Booking.Core.Repositories;
using Booking.Data.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;
        public IGymClassRepository GymClassRepository { get; private set; }
        public IApplicationUserRepository AppUserRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            GymClassRepository = new GymClassRepository(db);
            AppUserRepository = new ApplicationUserRepository(db);
        }

        public async Task CompleteAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
