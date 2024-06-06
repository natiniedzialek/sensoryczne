namespace HttpServer.Repositories;

using Microsoft.EntityFrameworkCore;
using Data.DbContext;
using Data.Models;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly ServerDbContext _dbContext;

    public MeasurementRepository(ServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddData(Measurement measurement)
    {
        try
        {
            await _dbContext.ConnectDatabase();
            await _dbContext.Measurements.AddAsync(measurement);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Measurement>> GetLastDataUpdate(Guid deviceId)
    {
        try
        {
            var fifteenMinutesAgo = DateTime.Now.AddMinutes(-15);

            return await _dbContext.Measurements
                .Where(m => m.DeviceId == deviceId && m.MeasureTime >= fifteenMinutesAgo)
                .ToListAsync();
        }
        catch (Exception)
        {
            return new List<Measurement>();
        }
    }
}