using Kawai.Api.DTOs;
using Kawai.Api.Models;
using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IUserRepository
{
    Task<List<UserDto>> GetAll(RequestParameter param);
    Task<List<UserDto>> GetDDL(string keyword);
    Task<List<JobPositionDto>> GetJobPositionDDL(string keyword);
    Task<UserDto> GetData(string user);
    Task Create(User user, string registerUser);
    Task Update(string userId, User user, string updateUser);
    Task Remove(string userId, string deleteUser);
    Task<Dictionary<string, object>> Capture(string userId);
    Task ChangePassword(string userId, string password);
}
