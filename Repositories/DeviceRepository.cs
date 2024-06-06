namespace HttpServer.Repositories;

using Data.DbContext;
using Data.Models;
using Microsoft.EntityFrameworkCore;

public class DeviceRepository : IDeviceRepository
{
    private readonly ServerDbContext _dbContext;

    public DeviceRepository(ServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<bool> AddDevice(Device device)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            await _dbContext.Devices.AddAsync(device);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Device?> GetDeviceByLogin(string login)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            return await _dbContext.Devices.FirstOrDefaultAsync(x => x.Login.Equals(login));
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<Device?> GetDeviceById(string id)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id.ToString().Equals(id));
            return device;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<Device>> GetUserDevices(string userId)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            
            if (!Guid.TryParse(userId, out var userGuid)) return new List<Device>();
            
            var user = await _dbContext.Users
                .Include(u => u.Devices)
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            return user?.Devices ?? new List<Device>();
        }
        catch (Exception)
        {
            return new List<Device>();
        }
    }
}