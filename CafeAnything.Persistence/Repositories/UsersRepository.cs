using CafeAnything.DomainModel;
using CafeAnything.DomainModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAnything.Persistence.Repositories
{
    public class UsersRepository : IUsers
    {
        private AppDbContext db = new AppDbContext();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // CRUD 

        public void AddUser(Users users)
        {
            db.Users.Add(users);
            Save();
        }

        public Users GetUser(int? id)
        {
            Users user = db.Users.Find(id);
            return user;
        }

        public IEnumerable<Users> GetUsers()
        {
            return db.Users.ToList();
        }

        public void UpdateUser(Users users)
        {
            db.Entry(users).State = EntityState.Modified;
            Save();
        }
        public void RemoveUser(Users users)
        {
            db.Users.Remove(users);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////

        public Users CheckUser(string username,Roles roles)
        {
            Users user = db.Users.Where(c => c.Username == username && c.Roles == roles).SingleOrDefault();
            return user;
        }
    }
}
