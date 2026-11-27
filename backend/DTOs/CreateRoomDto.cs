using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateRoomDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal Price { get; set; }

    [Range(1, 100)]
    public int Capacity { get; set; }
}