namespace HttpServer.Communication.Requests;

public class DisconnectDeviceRequest
{
    public string DeviceId { get; set; }
    
    public override string ToString()
    {
        return $"DisconnectDeviceRequest: DeviceId = {DeviceId}";
    }
}