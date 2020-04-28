using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Service.DataAccessLayer;
using Auth.Service.DTOs;
using Auth.Service.Model;

namespace Auth.Service.Repository
{
    public class UserDetails {
        public string id { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string[] events { get; set; }
        public int eventsCount { get { return events != null ? events.Count() : 0; } }
        public string feedbackStatus { get; set; }               
    }
    public class AuthRepository : IAuthRepository
    {
        //private readonly DataContext _context;
        private readonly IDocumentDBRepository<User> _documentDBContext;

        //public AuthRepository(DataContext context, IDocumentDBRepository<User> documentDBContext)
        //{
        //    _context = context;
        //    _documentDBContext = documentDBContext;
        //}
        public AuthRepository(IDocumentDBRepository<User> documentDBContext)
        {
            _documentDBContext = documentDBContext;
        }

        public async Task<User> Login(string username, string password, string role)
        {
            //var user = _context.User.FirstOrDefault(x => x.Username == username && x.Role == role);

            //User user = await _documentDBContext.GetItemAsync(username);
            var userResult = await _documentDBContext.GetItemsAsync(x => x.Username == username && x.Role == role);

            User user = userResult.ToList()[0];

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null; 

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public User Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //_context.User.Add(user);
            //_context.SaveChanges();
            _documentDBContext.CreateItemAsync(user);

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool UserExists(string username, string role)
        {
            //if (_context.User.Any(x => x.Username == username && x.Role == role))
            //    return true;
            if (_documentDBContext.GetItemsAsync(x => x.Username == username && x.Role == role).Result != null)
                return true;

            return false;
        }

        public async Task<UserForUpdateDto> UpdateUserDetails(UserForUpdateDto updatedUser) {
            var user = _documentDBContext.GetItemsAsync(x => x.id == updatedUser.id).Result.ToList()[0];
            if ( user != null) {
                user.Events = updatedUser.Events;
            }
            await _documentDBContext.UpdateItemAsync(updatedUser.id, user);
            return updatedUser;
        }

        public async Task<List<UserDetails>> GetAllUsers() {
            var users = await _documentDBContext.GetItemsAsync(x => x.id != null);
            
            List<UserDetails> userDetail = new List<UserDetails>();


            if (users == null)
                return null;

            users.ToList().ForEach(x => {
                UserDetails userObj = new UserDetails()
                {
                    id = x.id,
                    username = x.Username,
                    role = x.Role,
                    events = x.Events,
                    feedbackStatus = x.FeedbackStatus
                };

                userDetail.Add(userObj);
            });

            return userDetail;
        }
    }
}
