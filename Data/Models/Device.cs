using System.ComponentModel.DataAnnotations;

namespace HttpServer.Data.Models;

public class Device
{
    [Key]
    public Guid Id { get; set; }
    
    public string Login { get; set; }
    
    public string Password { get; set; }

    public string Name { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Key { get; set; }
    
    public string IV { get; set; }
    
    public List<Measurement> Measurements { get; set; } = new();
    
    public List<User> Users { get; set; } = new();
}