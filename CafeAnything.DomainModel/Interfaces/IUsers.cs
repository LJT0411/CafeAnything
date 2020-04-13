using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAnything.DomainModel.Interfaces
{
    public interface IUsers
    {
        IEnumerable<Users> GetUsers();

        Users GetUser(int? id);

        void AddUser(Users users);

        void UpdateUser(Users users);

        void RemoveUser(Users users);

        void Save();

        Users CheckUser(string username, Roles roles);
    }
}
