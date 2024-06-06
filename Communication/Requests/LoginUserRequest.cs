namespace HttpServer.Communication.Requests;

public class LoginUserRequest
{
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public override string ToString()
    {
        return $"LoginUserRequest: Username = {Username}";
    }
}