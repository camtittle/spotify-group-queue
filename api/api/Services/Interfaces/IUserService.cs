using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Create(string username);
        Task<User> FindByUsername(string username);
        Task<User> Find(string id);

    }
}
