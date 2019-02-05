using System.Threading.Tasks;
using api.Domain.Entities;

namespace api.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> Add(User user);
        Task<User> GetById(string id);
        Task<User> GetByUsername(string username);
        Task Update(User user);
    }
}