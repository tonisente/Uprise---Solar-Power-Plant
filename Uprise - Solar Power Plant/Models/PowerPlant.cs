using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Uprise___Solar_Power_Plant.Models;

public class PowerPlant
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [Required]
    public int Power { get; set; }

    [Required]
    public DateTime InstalationDate { get; set; }

    [Required]
    public double LocationLongitude { get; set; }

    [Required]
    public double LocationLatitude { get; set; }

    public ICollection<PowerPlantReading> PowerPlantReadings { get; set; } = null!;

    public object ToDTO()
    {
        return new
        {
            this.Id,
            this.Name,
            this.Power,
            this.InstalationDate,
            this.LocationLongitude,
            this.LocationLatitude,
            PowerPlantReadings = this.PowerPlantReadings
                .Select(r => new
                {
                    r.Id,
                    r.ReadAt,
                    r.PowerOutput
                })
        };
    }
}
