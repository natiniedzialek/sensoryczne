namespace HttpServer.Communication.Requests;

public class LoginDeviceRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    
    public override string ToString()
    {
        return $"LoginDeviceRequest: Login = {Login}, Password = {Password}";
    }
}