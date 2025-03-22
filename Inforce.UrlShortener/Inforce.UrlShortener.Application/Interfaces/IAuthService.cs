using FluentResults;
using Inforce.UrlShortener.Domain.Entities;
using Inforce.UrlShortener.Domain.Enums;

namespace Inforce.UrlShortener.Application.Interfaces;

public interface IAuthService
{
    Task<Result<User>> GetUser(Guid userId);
    Task<Result<string>> Login(string username, string password);
    Task<Result<string>> Register(string username, string password, Role role = Role.User);
}