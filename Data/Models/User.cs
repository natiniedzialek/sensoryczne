namespace HttpServer.Data.Models;

public class User
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public List<Device> Devices { get; set; } = new();
}