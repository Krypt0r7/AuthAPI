using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using AuthAPI.Helpers;

namespace API.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class UserService : IUserService
    { 
        private AppDbContext _context;
        public UserService(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;
            
            var user = _context.Users.SingleOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PassWordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public User Create(User user, string password)
        {
            if(string.IsNullOrEmpty(password))
                throw new AppException("Password is required");
            
            if(_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException($"Username {user.UserName} is already taken");

            byte[] passwordHash, PassWordSalt;

            CreatePasswordHash(password, out passwordHash, out PassWordSalt);

            user.PassWordHash = passwordHash;
            user.PasswordSalt = PassWordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if(user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                if(_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException($"Username {userParam.UserName} is already taken");
            }

            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;

                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PassWordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

         private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}