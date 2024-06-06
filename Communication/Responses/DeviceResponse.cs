using HttpServer.Data.Models;

namespace HttpServer.Communication.Responses;

public class DeviceResponse
{
    public string DeviceId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public List<DeviceMeasurement> Measurements { get; set; } = new();
    
    public string Message { get; set; }

    public DeviceResponse(string deviceId, string name, List<Measurement> measurements, string message)
    {
        DeviceId = deviceId;
        Name = name;
        Message = message;
        Measurements = measurements.Select(m => new DeviceMeasurement
        {
            MeasureTime = m.MeasureTime,
            PulseRate = m.PulseRate,
            Spo2 = m.Spo2,
            Temperature = m.Temperature
        }).ToList();
    }
    
    public override string ToString()
    {
        return $"DeviceResponse: " +
               $"DeviceId = {DeviceId}, " +
               $"Name = {Name}, " +
               $"Measurements = {Measurements.Count}";
    }
}