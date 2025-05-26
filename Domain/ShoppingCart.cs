namespace Domain;

public record ShoppingCartItem(Product Product, int Quantity);

public class ShoppingCart(List<ShoppingCartItem> cartItems) : List<ShoppingCartItem>;