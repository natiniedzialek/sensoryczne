using HttpServer.Communication.Requests;
using HttpServer.Communication.Responses;
using HttpServer.Repositories;
using HttpServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace HttpServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly ITokenService _tokenService;

    private readonly IUserRepository _userRepository;
    
    private readonly IDeviceRepository _deviceRepository;

    public AuthController(IUserRepository userRepository, ITokenService tokenService, IDeviceRepository deviceRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _deviceRepository = deviceRepository;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        Console.WriteLine("Register request received: " + request);

        if (UserExist(request.UserName))
        {
            Console.WriteLine("Sending response: \"User already exists\"");
            return BadRequest(new MessageResponse("User already exists"));
        }
        
        var registerResult = await _userRepository.AddUser(request.UserName, request.Password);

        if (!registerResult)
        {
            Console.WriteLine("Sending response: \"Cannot create user\"");
            return BadRequest(new MessageResponse("Cannot create user"));
        }
        
        Console.WriteLine("Sending response: " + "User was successfully created");
        return Ok(new MessageResponse("User was successfully created"));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginUserRequest request)
    {
        Console.WriteLine("Login request received: " + request);
        
        var user = await _userRepository.GetUser(request.Username);
        if (user is null)
        {
            Console.WriteLine("Sending response: \"User does not exist\"");
            return Unauthorized(new MessageResponse("User does not exist"));
        }
        
        var result = await _userRepository.CheckPassword(request.Username, request.Password);
        if (!result)
        {
            Console.WriteLine("Sending response: \"Incorrect password\"");
            return Unauthorized(new MessageResponse("Incorrect password"));
        }
        
        var token = _tokenService.GenerateToken(user.Id.ToString());
        var devices = await _deviceRepository.GetUserDevices(user.Id.ToString());
        var devicesData = devices.Select(device => new LoginResponse.DeviceData { DeviceId = device.Id, Name = device.Name, Key = device.Key, IV = device.IV }).ToList();

        var response = new LoginResponse
        {
            DevicesData = devicesData,
            Token = token
        };
        
        Console.WriteLine($"Sending token and deviceKeys for user {user.Username} login request");
        return Ok(response);
    }
    
    private bool UserExist(string username)
    {
        try
        {
            return _userRepository.GetUser(username).Result is not null;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
