using Microsoft.EntityFrameworkCore;
using Serilog;
using Uprise___Solar_Power_Plant.Data;
using Uprise___Solar_Power_Plant.Enums;
using Uprise___Solar_Power_Plant.Models;
using Uprise___Solar_Power_Plant.Services;

namespace Uprise___Solar_Power_Plant.Endpoints;

public static class PowerPlantEndpoints
{
    public static RouteGroupBuilder MapPowerPlantEndopoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (SolarPowerPlantDbContext context) =>
        {
            Log.Information("Getting all power plants");

            var powerPlants = await context.PowerPlants
                .Include(p => p.PowerPlantReadings)
                .ToListAsync();

            return Results.Ok(powerPlants.Select(p => p.ToDTO()));
        });

        group.MapGet("/{id:int}", async (SolarPowerPlantDbContext context, int id) =>
        {
            Log.Information($"Getting power plant {id}");

            var powerPlant = await context.PowerPlants
                .Include(p => p.PowerPlantReadings)
                .FirstOrDefaultAsync(p => p.Id == id);

            return powerPlant is not null ? Results.Ok(powerPlant.ToDTO()) : Results.NotFound();
        });

        group.MapPost("/", async (SolarPowerPlantDbContext context, PowerPlant newPowerPlant) =>
        {
            Log.Information("Creating new power plant");

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
            Log.Information($"Updating power plant {id}");

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
            Log.Information($"Deleting power plant {id}");

            var powerPlant = await context.PowerPlants.FindAsync(id);
            if (powerPlant is null)
            {
                return Results.NotFound();
            }

            context.PowerPlants.Remove(powerPlant);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapGet("/{id:int}/past-data", async (SolarPowerPlantDbContext dbContext, int id, DateTime? start, DateTime? end, ReadingGrouping? grouping) =>
        {
            Log.Information($"Fetching past reading for power plant {id}");
            var powerPlant = await dbContext.PowerPlants.FindAsync(id);

            // prerequirements
            {
                if (powerPlant is null)
                {
                    return Results.NotFound($"Power plant with id {id} doesn't exist");
                }

                if (start is null || end is null)
                {
                    return Results.BadRequest("Start and end timestamps are required (2025-02-23 14:30:45)");
                }

                if (start > end)
                {
                    return Results.BadRequest("Start timestamp is bigger than end timestamp");
                }
            }

            var results = dbContext.PowerPlantsReading.Where(r => r.PowerPlantId == id)
                                        .Where(r => r.ReadAt > start)
                                        .Where(r => r.ReadAt < end)
                                        .OrderBy(r => r.ReadAt)
                                        .Select(r => new PowerReadingDTO(r.ReadAt, r.PowerOutput))
                                        .ToList();

            if (grouping == ReadingGrouping.OneHour)
            {
                results = groupDataPerHour(results);
            }

            return Results.Ok(results);
        });

        group.MapGet("/{id:int}/future-data", async (SolarPowerPlantDbContext dbContext, int id, DateTime start, DateTime end, ReadingGrouping? grouping) =>
        {
            Log.Information($"Fetching future power predictions for power plant {id}");
            var powerPlant = await dbContext.PowerPlants.FindAsync(id);

            // prerequirements
            {
                if (powerPlant is null)
                {
                    return Results.NotFound($"Power plant with id {id} doesn't exist");
                }

                //if (start is null || end is null)
                //{
                //    return Results.BadRequest("Start and end timestamps are required (2025-02-23 14:30:45)");
                //}

                if (start > end)
                {
                    return Results.BadRequest("Start timestamp is bigger than end timestamp");
                }

                if (start < DateTime.Now)
                {
                    return Results.BadRequest("Time should be set in future");
                }
            }

            var results = new List<PowerReadingDTO>();
            var temperatures = await (new WeatherData()).GetWeatherData();
            var currentTime = start;
            while (currentTime < end)
            {
                var hourTime = currentTime.Date.AddHours(currentTime.Hour);
                var retrievedTemp = temperatures.TryGetValue(hourTime, out double temp);
                if (!retrievedTemp)
                {
                    break;
                }

                var powerPlantOutput = (int)(temp * powerPlant.Power);

                results.Add(new PowerReadingDTO(currentTime, powerPlantOutput));
                currentTime = currentTime.AddMinutes(15);
            }

            if (grouping == ReadingGrouping.OneHour)
            {
                results = groupDataPerHour(results);
            }

            return Results.Ok(results);
        });

        return group;
    }

    /// <summary>
    /// Expects sorted array....
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static List<PowerReadingDTO> groupDataPerHour(IEnumerable<PowerReadingDTO> data)
    {
        var aggregatedList = new List<PowerReadingDTO>();
        if (!data.Any())
        {
            return aggregatedList;
        }

        var currentHour = data.First().Timestamp.Date.AddHours(data.First().Timestamp.Hour);
        var currentSum = 0;
        foreach (var reading in data)
        {
            var readingHour = reading.Timestamp.Date.AddHours(reading.Timestamp.Hour);
            if (currentHour == readingHour)
            {
                currentSum += reading.Power;
            }
            else
            {
                
                aggregatedList.Add(new PowerReadingDTO(currentHour, currentSum));
                currentHour = readingHour;
                currentSum = reading.Power;
            }
        }

        // add last elements
        if (aggregatedList.Last().Timestamp == currentHour)
        {
            aggregatedList.Last().Power += currentSum;
        }
        else
        {
            aggregatedList.Add(new PowerReadingDTO( currentHour, currentSum));
        }

        return aggregatedList;
    }
}
