using HttpServer.Data.DbContext;
using HttpServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpServer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ServerDbContext _dbContext;
    
    public UserRepository(ServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddUser(string username, string password)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            await _dbContext.Users.AddAsync(new User
                {
                    Username = username,
                    Password = password
                });
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<User?> GetUser(string username)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<User?> GetUserById(string id)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString().Equals(id));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> CheckPassword(string username, string password)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
            return user != null && user.Password == password;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ConnectDeviceToUser(User user, Device device)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            var savedUser = await _dbContext.Users.FindAsync(user.Id);
            savedUser!.Devices.Add(device);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DisconnectDeviceFromUser(User user, Device device)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            var savedUser = await _dbContext.Users
                .Include(d => d.Devices)
                .FirstOrDefaultAsync(u => u.Id.ToString().Equals(user.Id.ToString()));
            var deviceToRemove = savedUser?.Devices.FirstOrDefault(d => d.Id.ToString().Equals(device.Id.ToString()));
            if (deviceToRemove is null || savedUser is null) return false;
            savedUser.Devices.Remove(deviceToRemove);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}