using System.ComponentModel.DataAnnotations;

namespace HttpServer.Data.Models;

public class Measurement
{
    [Key]
    public Guid Id { get; set; }
    public string Spo2 { get; set; } = string.Empty;
    public string PulseRate { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
    public DateTime MeasureTime { get; set; } = DateTime.MinValue;
    public Device Device { get; set; } = null!;
    public Guid DeviceId { get; set; }
}