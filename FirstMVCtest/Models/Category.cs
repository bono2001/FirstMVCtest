using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstMVCtest.Models
{
    public class Category
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Foreign key naar de Collection
        public int CollectionId { get; set; }

        // navigatie-eigenschap: de categorie hoort bij een Collection
        public Collection? Collection { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();

    }
}
