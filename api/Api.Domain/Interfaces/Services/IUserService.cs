using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> Create(string username);
    }
}
