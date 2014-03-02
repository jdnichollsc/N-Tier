using System;
using System.Collections.Generic;

namespace Entities
{
    public class Product
    {
        public Product()
        {
            UserProducts = new HashSet<UserProduct>();
        }
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int IdCategory { get; set; }
        public DateTime CreationDate { get; set; }
        public int Order { get; set; }
        public Category Category { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }
    }
}
