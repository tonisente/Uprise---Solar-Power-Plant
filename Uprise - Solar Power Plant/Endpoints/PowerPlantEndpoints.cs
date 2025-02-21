using Microsoft.EntityFrameworkCore;
using Uprise___Solar_Power_Plant.Data;
using Uprise___Solar_Power_Plant.Models;

namespace Uprise___Solar_Power_Plant.Endpoints;

public static class PowerPlantEndpoints
{
    public static RouteGroupBuilder MapPowerPlantEndopoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (SolarPowerPlantDbContext context) =>
        {
            var powerPlants = await context.PowerPlants
                .Include(p => p.PowerPlantReadings)
                .ToListAsync();

            return Results.Ok(powerPlants.Select(p => p.ToDTO()));
        });

        group.MapGet("/{id:int}", async (SolarPowerPlantDbContext context, int id) =>
        {
            var powerPlant = await context.PowerPlants
                .Include(p => p.PowerPlantReadings)
                .FirstOrDefaultAsync(p => p.Id == id);

            return powerPlant is not null ? Results.Ok(powerPlant.ToDTO()) : Results.NotFound();
        });

        group.MapPost("/", async (SolarPowerPlantDbContext context, PowerPlant newPowerPlant) =>
        {
            if (newPowerPlant is null)
            {
                return Results.BadRequest();
            }

            context.PowerPlants.Add(newPowerPlant);
            await context.SaveChangesAsync();

            return Results.Created($"/api/powerplants/{newPowerPlant.Id}", newPowerPlant);
        });

        group.MapPut("/{id:int}", async (SolarPowerPlantDbContext context, int id, PowerPlant updatedPowerPlant) =>
        {
            var powerPlant = await context.PowerPlants.FindAsync(id);
            if (powerPlant is null)
            {
                return Results.NotFound();
            }

            // TODO: use automapper or something smarter
            powerPlant.LocationLatitude = updatedPowerPlant.LocationLatitude;
            powerPlant.LocationLongitude = updatedPowerPlant.LocationLongitude;
            powerPlant.InstalationDate = updatedPowerPlant.InstalationDate;
            powerPlant.PowerPlantReadings = updatedPowerPlant.PowerPlantReadings;
            powerPlant.Power = updatedPowerPlant.Power;

            await context.SaveChangesAsync();

            return Results.NoContent();
        });


        group.MapDelete("/{id:int}", async (SolarPowerPlantDbContext context, int id) =>
        {
            var powerPlant = await context.PowerPlants.FindAsync(id);
            if (powerPlant is null)
            {
                return Results.NotFound();
            }

            context.PowerPlants.Remove(powerPlant);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });

        return group;
    }
}
