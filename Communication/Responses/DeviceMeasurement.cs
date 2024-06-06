namespace HttpServer.Communication.Responses;

public class DeviceMeasurement
{
    public string Spo2 { get; set; } = string.Empty;
    
    public string PulseRate { get; set; } = string.Empty;
    
    public string Temperature { get; set; } = string.Empty;

    public DateTime MeasureTime { get; set; } = DateTime.MinValue;
    
    public override string ToString()
    {
        return $"DeviceMeasurement: " +
               $"Spo2 = {Spo2}, " +
               $"PulseRate = {PulseRate}, " +
               $"Temperature = {Temperature}, " +
               $"MeasureTime = {MeasureTime:yyyy-MM-dd HH:mm:ss}";
    }
}