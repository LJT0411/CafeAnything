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
    public class CategoryRepository : ICategory
    {
        private AppDbContext db = new AppDbContext();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // CRUD 

        public void AddCategory(Categories category)
        {
            db.Categories.Add(category);
            Save();
        }

        public Categories GetCategory(int? id)
        {
            Categories category = db.Categories.Find(id);
            return category;
        }

        public IEnumerable<Categories> GetCategories()
        {
            return db.Categories.ToList();
        }

        public void UpdateCategory(Categories category)
        {
            db.Entry(category).State = EntityState.Modified;
            Save();
        }

        public void RemoveCategory(Categories category)
        {
            db.Categories.Remove(category);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
