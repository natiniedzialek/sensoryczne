namespace HttpServer.Communication.Requests;

public class ConnectDeviceRequest
{
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public override string ToString()
    {
        return $"ConnectDeviceRequest: Login = {Login}, Password = {Password}";
    }
}