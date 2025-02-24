using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using Uprise___Solar_Power_Plant.Data;
using Uprise___Solar_Power_Plant.Endpoints;
using Uprise___Solar_Power_Plant.Models;
using Uprise___Solar_Power_Plant.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SolarPowerPlantDbContext>(options =>
{
options.UseSqlServer(builder.Configuration.GetValue<string>("DB:ConnectionString"))
       .UseAsyncSeeding(async (context, _, ct) =>
       {    // seed Power Plants
           var shouldSeed = !await context.Set<PowerPlant>().AnyAsync();
            if (!shouldSeed)
            {
                return;
            }

            Random random = new Random();
            var faker = new Faker<PowerPlant>()
                .UseSeed(100)
                .RuleFor(x => x.Name, f => f.Person.FirstName)
                .RuleFor(x => x.InstalationDate, f => f.Person.DateOfBirth)
                .RuleFor(x => x.Power, f => random.Next(0, 100)) // Use the single instance
                .RuleFor(x => x.LocationLatitude, f => random.Next(-90, 90) * random.NextDouble())
                .RuleFor(x => x.LocationLongitude, f => random.Next(-180, 180) * random.NextDouble());

            var powerPlants = faker.Generate(builder.Configuration.GetValue<int>("Seeding:PowerPlants"));
            context.Set<PowerPlant>().AddRange(powerPlants);
            await context.SaveChangesAsync();
       })
       .UseAsyncSeeding(async (context, _, ct) => 
       {
           var shouldSeed = !await context.Set<PowerPlantReading>().AnyAsync();
           if (!shouldSeed)
           {
               return;
           }

           var powerPlants = await context.Set<PowerPlant>().ToListAsync();
           var random = new Random();
           var now = DateTime.UtcNow;

           foreach (var plant in powerPlants)
           {
               var startTime = now.AddDays(-random.Next(1, 30)) // Start any time in the past month
                                  .AddHours(-random.Next(1, 24)) // Randomize start hour
                                  .AddMinutes(-random.Next(1, 60)); // Randomize start minute

               var readings = Enumerable.Range(0, builder.Configuration.GetValue<int>("Seeding:ReadingsPerPlant"))
                   .Select(i => new PowerPlantReading
                   {
                       PowerPlant = plant,
                       ReadAt = startTime.AddMinutes(i * 15), // 15-minute intervals
                       PowerOutput = random.Next(0, 100) // Random power output
                   })
                   .ToList();

               await context.Set<PowerPlantReading>().AddRangeAsync(readings);
           }

           await context.SaveChangesAsync(ct);
       })
       .UseSeeding((context, _) =>
        {    // seed Power Plants
            var shouldSeed = !context.Set<PowerPlant>().Any();
            if (!shouldSeed)
            {
                return;
            }

            Random random = new Random();
            var faker = new Faker<PowerPlant>()
                .UseSeed(100)
                .RuleFor(x => x.Name, f => f.Person.FirstName)
                .RuleFor(x => x.InstalationDate, f => f.Person.DateOfBirth)
                .RuleFor(x => x.Power, f => random.Next(0, 100)) // Use the single instance
                .RuleFor(x => x.LocationLatitude, f => random.Next(-90, 90) * random.NextDouble())
                .RuleFor(x => x.LocationLongitude, f => random.Next(-180, 180) * random.NextDouble());

            var powerPlants = faker.Generate(builder.Configuration.GetValue<int>("Seeding:PowerPlants"));
            context.Set<PowerPlant>().AddRange(powerPlants);
            context.SaveChanges();
        })
       .UseSeeding((context, _) =>
       {
           var shouldSeed = !context.Set<PowerPlantReading>().Any();
           if (!shouldSeed)
           {
               return;
           }

           var powerPlants = context.Set<PowerPlant>().ToList();
           var random = new Random();
           var now = DateTime.UtcNow;

           foreach (var plant in powerPlants)
           {
               var startTime = now.AddDays(-random.Next(1, 30)) // Start any time in the past month
                                  .AddHours(-random.Next(1, 24)) // Randomize start hour
                                  .AddMinutes(-random.Next(1, 60)); // Randomize start minute
               var readings = Enumerable.Range(0, builder.Configuration.GetValue<int>("Seeding:ReadingsPerPlant"))
                   .Select(i => new PowerPlantReading
                   {
                       PowerPlant = plant,
                       ReadAt = startTime.AddMinutes(i * 15), // 15-minute intervals
                       PowerOutput = random.Next(0, 100) // Random power output
                   })
                   .ToList();

               context.Set<PowerPlantReading>().AddRange(readings);
           }

           context.SaveChanges();
       });
});

builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:JWT:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("AppSettings:JWT:Audience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:JWT:Secret"))),
        };
    });

var app = builder.Build();


// Logger configuration
var file = builder.Configuration.GetValue<string>("Logging:FilePath");
Log.Logger = (Serilog.ILogger)new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        file,
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

// preparation for seeding
await using (var serviceScope = app.Services.CreateAsyncScope())
await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<SolarPowerPlantDbContext>())
{
    await dbContext.Database.EnsureCreatedAsync();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGroup("/api/powerplant").MapPowerPlantEndopoints().RequireAuthorization();
//app.MapGroup("/api/powerplant").MapPowerPlantEndopoints();
app.MapGroup("/api/user").MapUserEndpoints();

app.Run();

