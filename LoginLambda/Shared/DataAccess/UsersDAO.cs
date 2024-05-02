using System.Threading.Tasks;
using LoginLambda.Models;

namespace Shared.DataAccess
{
    public interface IUsersDAO
    {
        UserContext? GetUser(string id);

        Task PutUser(UserContext user);

        Task DeleteUser(string id);

        Task<UserWrapper> GetAllUsers();
    }
}