using ZRoomLoginLibrary.Models;

namespace ZRoomLoginLibrary.Repositories
{
    public interface IUserRepository
    {
        User Authenticate(LoginDTO loginCreds);
    }
}