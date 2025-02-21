using System.ComponentModel.DataAnnotations;

namespace Uprise___Solar_Power_Plant.Models;

public class PowerPlantReading
{
    public int Id { get; set; }

    public DateTime ReadAt { get; set; }

    [Required]
    public int PowerOutput { get; set; }

    public int PowerPlantId { get; set; }

    public PowerPlant PowerPlant { get; set; } = null!;
}