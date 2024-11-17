using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Infrastructure.Data;

namespace Cofidis.Credit.Infrastructure.Repositories
{
    public class UserRepository(CreditDbContext db) : Repository<User>(db), IUserRepository
    {
    }
}
