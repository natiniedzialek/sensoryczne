using HttpServer.Communication.Requests;
using HttpServer.Communication.Responses;
using HttpServer.Data.Models;
using HttpServer.Repositories;
using HttpServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HttpServer.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class DeviceController : Controller
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IMeasurementRepository _measurementRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IDataFilterService _dataFilterService;

    public DeviceController(IDeviceRepository deviceRepository, ITokenService tokenService,
        IUserRepository userRepository, IMeasurementRepository measurementRepository,
        IEncryptionService encryptionService, IDataFilterService dataFilterService)
    {
        _deviceRepository = deviceRepository;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _measurementRepository = measurementRepository;
        _encryptionService = encryptionService;
        _dataFilterService = dataFilterService;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterDeviceResponse>> Register([FromBody] RegisterDeviceRequest request)
    {
        Console.WriteLine("Register device request received: " + request);

        if (await _deviceRepository.GetDeviceByLogin(request.Login) is not null)
        {
            Console.WriteLine("Sending response: " + "Device with given login is already registered");
            return BadRequest("Device with given login is already registered");
        }

        var (key, iv) = _encryptionService.GenerateKeyAndIV();
        
        var device = new Device
        {
            Name = request.Name,
            Login = request.Login,
            Password = request.Password,
            RegistrationDate = DateTime.Now,
            Key = key,
            IV = iv
        };

        var registrationResult = await _deviceRepository.AddDevice(device);

        if (!registrationResult)
        {
            Console.WriteLine("Sending response: " + "The device could not be registered");
            return BadRequest("The device could not be registered");
        }

        var savedDevice = await _deviceRepository.GetDeviceByLogin(request.Login);
        var token = _tokenService.GenerateToken(savedDevice!.Id.ToString());

        var response = new RegisterDeviceResponse
        {
            Token = token,
            Key = key,
            IV = iv
        };

        Console.WriteLine("Sending token response: " + "Device successfully registered");
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RegisterDeviceResponse>> Login([FromBody] LoginDeviceRequest request)
    {
        Console.WriteLine("Login device request received: " + request);

        var device = await _deviceRepository.GetDeviceByLogin(request.Login);
        if (device is null)
        {
            Console.WriteLine("Sending response: " + "The device does not exist");
            return BadRequest(new MessageResponse("The device does not exist"));
        }
        
        if (request.Password != device.Password)
        {
            Console.WriteLine("Sending response: " + "Wrong device password");
            return BadRequest(new MessageResponse("Wrong device password"));
        }

        var token = _tokenService.GenerateToken(device.Id.ToString());

        var response = new RegisterDeviceResponse
        {
            Token = token,
            Key = device.Key,
            IV = device.IV
        };

        Console.WriteLine("Sending token response: " + $"Device {device.Name} logged in.");
        return Ok(response);
    }
    
    [HttpPost("connect")]
    public async Task<ActionResult<LoginResponse.DeviceData>> Connect([FromBody] ConnectDeviceRequest request)
    {
        Console.WriteLine("Connect request received: " + request);
        
        var user = await GetUserFromToken(HttpContext.Request.Headers);
        if (user is null)
        {
            Console.WriteLine("Sending response: " + "User does not exist");
            return Unauthorized(new MessageResponse("User does not exist"));
        }

        var device = await _deviceRepository.GetDeviceByLogin(request.Login);
        if (device is null)
        {
            Console.WriteLine("Sending response: " + "The device does not exist");
            return BadRequest(new MessageResponse("The device does not exist"));
        }
        
        if (request.Password != device.Password)
        {
            Console.WriteLine("Sending response: " + "Wrong device password");
            return BadRequest(new MessageResponse("Wrong device password"));
        }

        var result = await _userRepository.ConnectDeviceToUser(user, device);
        if (!result)
        {
            Console.WriteLine("Sending response: " + "Device could not be connected");
            return BadRequest(new MessageResponse("Device could not be connected"));
        }

        Console.WriteLine("Sending response: " + "Device successfully connected");
        return Ok(new LoginResponse.DeviceData
        {
            DeviceId = device.Id,
            Name = device.Name,
            Key = device.Key,
            IV = device.IV
        });
    }

    [HttpDelete("disconnect")]
    public async Task<IActionResult> Disconnect([FromBody] DisconnectDeviceRequest request)
    {
        Console.WriteLine("Remove request received: " + request);
        
        var user = await GetUserFromToken(HttpContext.Request.Headers);
        if (user is null)
        {
            Console.WriteLine("Sending response: " + "User does not exist");
            return Unauthorized(new MessageResponse("User does not exist"));
        }

        var device = await _deviceRepository.GetDeviceById(request.DeviceId);
        if (device is null)
        {
            Console.WriteLine("Sending response: " + "The device does not exist");
            return BadRequest(new MessageResponse("The device does not exist"));
        }
        
        var result = await _userRepository.DisconnectDeviceFromUser(user, device);
        if (!result)
        {
            Console.WriteLine("Sending response: " + "Device could not be disconnected");
            return BadRequest(new MessageResponse("Device could not be disconnected"));
        }

        Console.WriteLine("Sending response: " + "Device successfully disconnected");
        return Ok(new MessageResponse("Device successfully disconnected"));
    }

    [HttpPost("data")]
    public async Task<IActionResult> AddData([FromBody] AddDataRequest request)
    {
        Console.WriteLine("AddData request received: " + request);
        
        var device = await GetDeviceFromToken(HttpContext.Request.Headers);
        if (device is null)
        {
            Console.WriteLine("Sending response: " + "Device does not exist");
            return Unauthorized(new MessageResponse("Device does not exist"));
        }
        

        var result = await _measurementRepository.AddData(
            new Measurement
            {
                Spo2 = request.Spo2,
                PulseRate = request.PulseRate,
                Temperature = request.Temperature,
                MeasureTime = DateTime.Now,
                DeviceId = device.Id 
            });
        
        if (!result)
        {
            Console.WriteLine("Sending response: " + "Data could not be added");
            return BadRequest(new MessageResponse("Data could not be added"));
        }

        Console.WriteLine("Sending response: " + "Data successfully added");
        return Ok(new MessageResponse("Data successfully added"));
    }
    
    [HttpGet("list")]
    public async Task<ActionResult<DeviceListResponse>> ListDevices()
    {
        Console.WriteLine("List request received");
        
        var user = await GetUserFromToken(HttpContext.Request.Headers);
        if (user is null)
        {
            Console.WriteLine("Sending response: " + "User does not exist");
            return Unauthorized(new MessageResponse("User does not exist"));
        }

        var devices = await _deviceRepository.GetUserDevices(user.Id.ToString());
        
        if (devices.IsNullOrEmpty())
        {
            Console.WriteLine("Sending response: " + "User does not have any devices");
            return Ok(new DeviceListResponse(new List<DeviceResponse>()));
        }

        List<DeviceResponse> response = new();
        foreach (var device in devices) 
        {
            var measurements = await _measurementRepository.GetLastDataUpdate(device.Id);
            var message = _dataFilterService.GenerateMessage(measurements, device.Key, device.IV);
            response.Add(new DeviceResponse(device.Id.ToString(), device.Name, measurements.OrderBy(m => m.MeasureTime).ToList(), message));
        }

        Console.WriteLine("Sending response: " + response);
        return Ok(new DeviceListResponse(response));
    }

    private async Task<User?> GetUserFromToken(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue("Authorization", out var authHeader) || !authHeader.ToString().StartsWith("Bearer "))
        {
            return null;
        }

        var token = authHeader.ToString()["Bearer ".Length..].Trim();

        var userId = _tokenService.GetId(token);

        return userId != null ? await _userRepository.GetUserById(userId) : null;
    }

    private async Task<Device?> GetDeviceFromToken(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue("Authorization", out var authHeader) || !authHeader.ToString().StartsWith("Bearer "))
        {
            return null;
        }

        var token = authHeader.ToString()["Bearer ".Length..].Trim();

        var deviceId = _tokenService.GetId(token);

        return deviceId != null ? await _deviceRepository.GetDeviceById(deviceId) : null;
    }
}