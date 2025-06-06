using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel;

public class CreateProductViewModel
{
    [Required] [MaxLength(256)] public string Name { get; set; } = string.Empty;

    [Required] [MaxLength(256)] public string ShortDescription { get; set; } = string.Empty;

    [Required] [MaxLength(4096)] public string LongDescription { get; set; } = string.Empty;

    [Required] [Range(0, double.MaxValue)] public double Price { get; set; } = 0;

    [Required] public bool Listed { get; set; } = true;

    [Required] [Range(0, int.MaxValue)] public int Stock { get; set; } = 0;
}