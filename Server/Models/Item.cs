using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class Item
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}