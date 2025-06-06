using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.ViewModel;

public class ViewShoppingCartViewModel
{
    [Required] public IEnumerable<CartItemViewModel> Items = new HashSet<CartItemViewModel>();
}

public class CartItemViewModel
{
    [Required] [HiddenInput] public Guid ProductId { get; init; }
    [Required] [MaxLength(256)] public string ProductName { get; init; } = "";
    [Required] [Range(1, int.MaxValue)] public int Count { get; init; } = 1;
    [Required] public decimal Price { get; init; } = 0;
    [Required] public int Stock { get; init; } = 0;
}