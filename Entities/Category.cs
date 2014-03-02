using System.Collections.Generic;

namespace Entities
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
