using System;
using System.Collections.Generic;

namespace Entities
{
    public class User
    {
        public User()
        {
            UserProducts = new HashSet<UserProduct>();
        }
        public int IdUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Document { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }
    }
}
