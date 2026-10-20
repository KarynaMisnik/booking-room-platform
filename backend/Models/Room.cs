namespace backend.Models;

public class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Capacity { get; set; }
}