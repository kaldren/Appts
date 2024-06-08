using Appts.Features.Identity.Models;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[HttpPost("api/identity/login")]
[AllowAnonymous]
public class LoginUserEndpoint : Endpoint<LoginUserModel,
                                   Results<Ok<LoginUserResponseModel>,
                                           NotFound,
                                           ProblemDetails>>
{
    private readonly IMediator _mediator;

    public LoginUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<Results<Ok<LoginUserResponseModel>,
                                           NotFound,
                                           ProblemDetails>> ExecuteAsync(LoginUserModel request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));

        if (result)
        {
            return TypedResults.Ok(new LoginUserResponseModel("OK"));
        }
        else
        {
            AddError("Invalid email or password");
        }

        return new FastEndpoints.ProblemDetails(ValidationFailures);
    }
}

public record LoginUserModel(string Email, string Password);

public record LoginUserResponseModel(string Token);

public record LoginUserCommand(LoginUserModel model) : IRequest<bool>;

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, bool>
{
    private UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public LoginUserCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IPublisher publisher, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _publisher = publisher;
        _configuration = configuration;
    }


    public async Task<bool> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.model.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.model.Password))
        {
            await _signInManager.SignInAsync(user, false, null);
            return true;
        }

        return false;
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public static class Settings
{
    public static string JwtIssuer { get; } = "appts.com";
    public static string JwtKey { get; } = "Ek]3U@mFrhyhTu70FUB(yg!b.3j*/(FKzFKL+)[_RQvf?_6jdY";
}