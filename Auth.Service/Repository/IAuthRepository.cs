using Auth.Service.DTOs;
using Auth.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.Repository
{
    public interface IAuthRepository
    {
        User Register(User user, string password);

        Task<User> Login(string username, string password, string role);

        bool UserExists(string username, string role);

        Task<UserForUpdateDto> UpdateUserDetails(UserForUpdateDto updatedUser);

        Task<List<UserDetails>> GetAllUsers();
    }
}
