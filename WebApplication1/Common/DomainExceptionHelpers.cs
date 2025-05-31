using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication1.Common;

public static class DomainExceptionHelpers
{
    /// <summary>
    /// Will add all errors in the DomainException dictionary, and put it into the ModelStateDictionary dict.
    /// </summary>
    public static void AddDomainExceptions(this ModelStateDictionary dict, DomainException e)
    {
        dict.Clear();
        foreach (var (k, v) in e.DomainExceptions)
        {
            dict.TryAddModelError(k, v);
        }
    }
}