using HttpServer.Data.Models;

namespace HttpServer.Repositories;

public interface IUserRepository
{
    public Task<bool> AddUser(string username, string password);
    
    public Task<User?> GetUser(string username);
    
    public Task<User?> GetUserById(string id);
    
    public Task<bool> CheckPassword(string username, string password);

    public Task<bool> ConnectDeviceToUser(User user, Device device);

    public Task<bool> DisconnectDeviceFromUser(User user, Device device);
}