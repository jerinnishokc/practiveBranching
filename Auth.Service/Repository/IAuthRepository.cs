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

        User Login(string username, string password);

        bool UserExists(string username, string role);
    }
}
