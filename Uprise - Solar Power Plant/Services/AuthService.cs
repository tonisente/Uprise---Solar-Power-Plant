using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uprise___Solar_Power_Plant.Data;
using Uprise___Solar_Power_Plant.Models;
using Uprise___Solar_Power_Plant.Security;

namespace Uprise___Solar_Power_Plant.Services;

public class AuthService(SolarPowerPlantDbContext dbContext, IConfiguration config)
{
    public async Task<string?> LoginAsync(UserDTO userDTO)
    {
        // get user with the username
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(userDTO.Username));

        if (user == null)
        {
            return null;
        }

        // check password
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userDTO.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        string token = new JWTTokenGenerator().GenerateJWTToken(config, user);
        return token; ;
    }

    public async Task<User?> RegisterAsync(UserDTO userDTO)
    {
        // check if user already exists
        if (await dbContext.Users.AnyAsync(u => u.Username == userDTO.Username))
        {
            return null; // TODO: add better signaling
        }

        var user = new User();
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, userDTO.Password);
        user.Username = userDTO.Username;
        user.PasswordHash = hashedPassword;

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}
