namespace HttpServer.Communication.Requests;

public class RegisterDeviceRequest
{
    public string Login { get; set; }

    public string Password { get; set; }
    
    public string Name { get; set; }
    
    public override string ToString()
    {
        return $"RegisterDeviceRequest: Login = {Login}, " +
               $"Password = {Password}, " +
               $"Name = {Name}";
    }
}