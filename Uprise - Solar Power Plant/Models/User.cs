using System.ComponentModel.DataAnnotations;

namespace Uprise___Solar_Power_Plant.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;
}
