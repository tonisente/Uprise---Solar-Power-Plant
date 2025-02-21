using Microsoft.EntityFrameworkCore;
using Uprise___Solar_Power_Plant.Models;

namespace Uprise___Solar_Power_Plant.Data;

public class SolarPowerPlantDbContext(DbContextOptions<SolarPowerPlantDbContext> options) : DbContext(options)
{
    public DbSet<PowerPlant> PowerPlants { get; set; } = null!;

    public DbSet<PowerPlantReading> PowerPlantsReading { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;
}
