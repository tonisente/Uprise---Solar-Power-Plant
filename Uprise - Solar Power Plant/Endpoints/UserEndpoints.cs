using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Uprise___Solar_Power_Plant.Data;
using Uprise___Solar_Power_Plant.Models;
using Uprise___Solar_Power_Plant.Security;
using Uprise___Solar_Power_Plant.Services;

namespace Uprise___Solar_Power_Plant.Endpoints;

public static class UserEndpoints
{

    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (UserDTO userDTO, AuthService authService) =>
        {
            if (userDTO == null)
            {
                return Results.BadRequest();
            }

            var user = await authService.RegisterAsync(userDTO);

            if (user == null)
            {
                return Results.BadRequest();
            }

            return Results.Ok(user);
        });

        group.MapPost("/login", async (UserDTO userDTO, AuthService authService) =>
        {
            if (userDTO == null)
            {
                return Results.BadRequest();
            }

            var jwtToken = await authService.LoginAsync(userDTO);
            if (jwtToken == null)
            {
                return Results.BadRequest();
            }

            return Results.Ok(jwtToken);
        });

        group.MapGet("/authTest", () =>
        {
            return Results.Ok("Authenticated!");
        }).RequireAuthorization();

        return group;
    }
}
