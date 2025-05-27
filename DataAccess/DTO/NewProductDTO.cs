namespace DataAccess.DTO;

public class NewProductDTO(
    string name,
    string shortDescription,
    string longDescription,
    double price)
{
    public string Name { get; set; } = name;
    public string ShortDescription { get; set; } = shortDescription;
    public string LongDescription { get; set; } = longDescription;
    public double Price { get; set; } = price;

    public NewProductDTO() : this(string.Empty, string.Empty, string.Empty, 0)
    {

    }
}