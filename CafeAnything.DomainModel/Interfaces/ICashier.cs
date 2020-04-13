using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAnything.DomainModel.Interfaces
{
    public interface ICashier
    {
        IEnumerable<Tables> GetTables();

        Tables GetTable(int? id);

        void AddTable(Tables table);

        void UpdateTable(Tables table);

        void RemoveTable(Tables table);

        void Save();
    }
}
