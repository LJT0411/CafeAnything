using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAnything.DomainModel.Interfaces
{
    public interface ICategory
    {
        IEnumerable<Categories> GetCategories();

        Categories GetCategory(int? id);

        void AddCategory(Categories category);

        void UpdateCategory(Categories category);

        void RemoveCategory(Categories category);

        void Save();
    }
}
