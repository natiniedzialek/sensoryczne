namespace HttpServer.Communication.Responses;

public class RegisterDeviceResponse
{
    public string Token { get; set; }
    public string Key { get; set; }
    public string IV { get; set; }
}