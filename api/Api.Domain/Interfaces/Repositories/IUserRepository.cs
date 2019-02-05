using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<User> GetById(string id);
        Task<User> GetByUsername(string username);
        Task Update(User user);
    }
}