using System.Threading.Tasks;

namespace Booking.Core.Repositories
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository AppUserRepository { get; }
        IGymClassRepository GymClassRepository { get; }

        Task CompleteAsync();
    }
}