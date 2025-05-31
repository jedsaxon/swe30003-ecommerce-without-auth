namespace DataAccess.DTO;

public record ProductDTO(Guid ProductId, string Name, string ShortDescription, string LongDescription, double Price, bool Listed);