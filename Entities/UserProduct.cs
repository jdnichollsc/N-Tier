using System;

namespace Entities
{
    public class UserProduct
    {
        public int IdUserProduct { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }
        public int IdProduct { get; set; }
        public Product Product { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
