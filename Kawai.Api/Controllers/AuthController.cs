using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Kawai.Api.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : HahaController
{
    private readonly IUserRepository _userRepository;
    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignIn model)
    {
        var sman = GetService<SessionManager>();

        var session = await sman.Authenticate(model.UserName, model.Password);

        var results = await _userRepository.GetData(model.UserName);

        if (!session.IsSucceeded)
            return Invalid(message: "Invalid username/password");

        HttpContextAccessor.HttpContext.Response.Cookies.Append("__SIDX", session.Token, new CookieOptions
        {
            HttpOnly = false, // Set HttpOnly
            Secure = true,   // Set Secure
            SameSite = SameSiteMode.Strict // Set SameSite
        });

        Stream image = FileStorage.GetFromImages(results.ImageName);
        byte[] imageByte = null;
        string imageBase64 = "";
        using (MemoryStream memoryStream = new MemoryStream())
        {
            image.CopyTo(memoryStream);
            imageByte = memoryStream.ToArray();
            //imageBase64 = Convert.ToBase64String(memoryStream.ToArray());
        }
        image.Dispose();

        return Success(new
        {
            AccessToken = session.Token,
            UserPhoto = imageByte,
            //UserPhoto64 = imageBase64,
            ExpiryDate = session.Session.ExpiryDate.Value,
        });
    }
}
