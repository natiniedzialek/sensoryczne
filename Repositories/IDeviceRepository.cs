namespace HttpServer.Repositories;

using Data.Models;

public interface IDeviceRepository
{
    Task<bool> AddDevice(Device device);

    Task<Device?> GetDeviceByLogin(string login);
    
    Task<Device?> GetDeviceById(string id);
    
    Task<List<Device>> GetUserDevices(string userId);
}