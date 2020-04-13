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
    public class CashierRepository : ICashier
    {
        private AppDbContext db = new AppDbContext();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // CRUD 

        public void AddTable(Tables table)
        {
            db.Tables.Add(table);
            Save();
        }

        public Tables GetTable(int? id)
        {
            Tables table = db.Tables.Find(id);
            return table;
        }

        public IEnumerable<Tables> GetTables()
        {
            return db.Tables.ToList();
        }

        public void UpdateTable(Tables table)
        {
            db.Entry(table).State = EntityState.Modified;
            Save();
        }

        public void RemoveTable(Tables table)
        {
            db.Tables.Remove(table);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
