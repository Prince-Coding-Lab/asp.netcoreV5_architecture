using ScaffoldNetCore.Core.Dto;
using ScaffoldNetCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldNetCore.Core.Interfaces
{
    public interface IUserService
    {
        Task<DatabaseResponse> CreateUserAsync(Users user);
        Task<UserDto> AuthenticateAsync(string username, string password);
    }
}
