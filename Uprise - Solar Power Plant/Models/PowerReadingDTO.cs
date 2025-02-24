namespace Uprise___Solar_Power_Plant.Models;

public class PowerReadingDTO
{
    public DateTime Timestamp { get; set; }
    public int Power { get; set; }

    public PowerReadingDTO(DateTime timestamp, int power)
    {
        Timestamp = timestamp;
        Power = power;
    }
}

