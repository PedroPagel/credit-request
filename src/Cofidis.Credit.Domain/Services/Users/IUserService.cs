using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Models.Users;

namespace Cofidis.Credit.Domain.Services.Users
{
    public interface IUserService
    {
        Task<User> AddUser(UserRequest request);
        Task<User> AddUserByDigitalKey(string nif, decimal monthlyIncome);
        Task<User> UpdateUser(Guid userId, UserRequest request);
        Task<User> GetUserById(Guid id);
        Task<User> GetUserByNif(string nif);
        Task<bool> DeleteUser(Guid id);
    }
}
