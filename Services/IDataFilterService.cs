using HttpServer.Data.Models;

namespace HttpServer.Services;

public interface IDataFilterService
{
    public string GenerateMessage(List<Measurement> measurements, string Key, string IV);
    public bool IsMeasurementCorrect(List<Measurement> measurements, Measurement newMeasurement, string Key, string IV);
}