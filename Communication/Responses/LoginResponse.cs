namespace HttpServer.Communication.Responses;

public class LoginResponse
{
    public string Token { get; set; }
    
    public List<DeviceData> DevicesData { get; set; }

    public class DeviceData
    {
        public Guid DeviceId { get; set; }
        
        public string Name { get; set; }
        
        public string Key { get; set; }
        
        public string IV { get; set; }
    }
}