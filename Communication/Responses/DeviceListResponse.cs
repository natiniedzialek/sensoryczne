namespace HttpServer.Communication.Responses;

public class DeviceListResponse
{
    public List<DeviceResponse> DeviceList { get; set; }

    public DeviceListResponse(List<DeviceResponse> list) => DeviceList = list;
    
    
    public override string ToString()
    {
        return $"DeviceListResponse: DeviceList = {DeviceList}";
    }
}