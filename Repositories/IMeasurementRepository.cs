namespace HttpServer.Repositories;

using Data.Models;

public interface IMeasurementRepository
{
    Task<bool> AddData(Measurement measurement);

    Task<List<Measurement>> GetLastDataUpdate(Guid deviceId);
}