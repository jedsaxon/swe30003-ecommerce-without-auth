using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.ViewModel;

public class EditProductViewModel
{
    [Required] [HiddenInput] public Guid ProductId { get; set; } = Guid.Empty;

    [Required] [MaxLength(256)] public string Name { get; set; } = string.Empty;

    [Required] [MaxLength(256)] public string ShortDescription { get; set; } = string.Empty;

    [Required] [MaxLength(4096)] public string LongDescription { get; set; } = string.Empty;

    [Required] [Range(0, double.MaxValue)] public decimal Price { get; set; }

    [Required] public bool Listed { get; set; }
}