namespace Devices.Application.Interfaces;

public interface IAuthService
{
    string GenerateToken(string username);
}