using IdentityService.Api.Data;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Api.Models;

public class UsersSeed(ILogger<UsersSeed> logger, UserManager<ApplicationUser> userManager) : IDbSeeder<ApplicationDbContext>
{
    public async Task SeedAsync(ApplicationDbContext context)
    {
        var unalman = await userManager.FindByNameAsync("unalman");

        if (unalman == null)
        {
            unalman = new ApplicationUser
            {
                UserName = "unalman",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
                CardHolderName = "Alice Smith",
                CardNumber = "XXXXXXXXXXXX1881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Expiration = "12/24",
                Id = Guid.NewGuid().ToString(),
                LastName = "Smith",
                Name = "unal",
                PhoneNumber = "1234567890",
                ZipCode = "98052",
                State = "WA",
                Street = "15703 NE 61st Ct",
                SecurityNumber = "123"
            };

            var result = await userManager.CreateAsync(unalman, "Pass123$");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("unalman created");
        }
        else
            if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("unalman already exists");

        var bob = await userManager.FindByNameAsync("bob");
        if (bob == null)
        {
            bob = new ApplicationUser
            {
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true,
                CardHolderName = "Bob Smith",
                CardNumber = "XXXXXXXXXXXX1881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Expiration = "12/24",
                Id = Guid.NewGuid().ToString(),
                LastName = "Smith",
                Name = "Bob",
                PhoneNumber = "1234567890",
                ZipCode = "98052",
                State = "WA",
                Street = "15703 NE 61st Ct",
                SecurityNumber = "456"
            };

            var result = await userManager.CreateAsync(bob, "Pass123$");
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("bob created");
        }
        else
            if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("bob already exists");
    }
}
