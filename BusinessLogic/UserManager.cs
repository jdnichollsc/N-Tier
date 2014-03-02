using DataAccess;
using Entities;

namespace BusinessLogic
{
    public class UserManager
    {
        public User GetUserById(int IdUser)
        {
            using (var repository = new GenericRepository())
            {
                return repository.ReadById<User>(IdUser);
            }
        }
    }
}
