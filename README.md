# ECommerce site for SWE30003

Just your generic, boring, standard ecommerce site for the SWE30003 software architectures unit.

## Development

The program will automatically create the database and seed it with values on startup, at 
`WebApplication1/app.db`. 

## Architecture

The project has four main layers. The domain layer, service layer, data-access layer, and 
application layer. 

### Domain Layer

The domain layer, under `Domain/`, is its own separate library and should not ever communicate
with any other parts of the project. Each class that has to be stored in the database, will
have the following requirements:

 - its own optional `Guid` property to store its ID
 - It marks the constructor `private`. To construct the object, 2 `public static` factory methods 
   are created that either create a new object (without an id argument) that performs validation,
   and one that comes from the database that does have an ID argument. See the `Product` class.
 - Most properties should be decorated with `{ get; private set; }`. This means instead of directly
   exposing the property's set function, create a custom one. 
 - Those 'mutation' methods should have descriptive names. Instead of `ChangeName()`, do `Rename()`

### Data Access Layer

The data access layer is where the SQLite database is implemented. The repositories directory
contains the main repository interfaces that expose the methods needed to alter the database.

Their arguments are always DTOs (data transfer objects), as this layer does not reference 
the domain layer. It would be an anti-pattern to have the database layer directly call methods in
the domain layer.

Seperate classes under `DataAccess/Repositories/Sqlite` implement those repository interfaces, 
and are configured in `WebApplication1/Program.cs`, as such:

```csharp
builder.Services.AddTransient<IProductRepository, SqliteProductsRepository>();
```

Then, the service layer can inject those classes without having to create SQLite implementations
of it manually. For example:

```csharp
// Here we inject `IProductRepository` instead of the Sqlite implementation
public class ProductsService(IProductRepository productRepository)
{
    public async Task<Product[]> GetAllProducts(bool includeUnlisted = false)
    {
        return (await productRepository
            .GetProducts(includeUnlisted))
            .Select(s => Product.ProductWithIdentity(
                s.ProductId,
                s.Name,
                s.ShortDescription,
                s.LongDescription,
                (decimal)s.Price,
                s.Listed
            ))
            .ToArray();
    }
    // ...
}
```

### Service Layer

This layer reference connects the application layer to the data-access layer and domain layer
respectively. Because its connecting the domain layer, it can perform extra validation and
verification to ensure that operations are correctly done. 

For example, the EditProduct class, upon changing data in the domain layer, also needs
to check if the product actually exists in the database before modifying the data.

The usual process of a method inside a service class is to 

1. Get the data from the data access layer
2. Convert the DTOs into domain-layer classes
3. Modify those domain classes by calling their exposed methods
4. Convert the modified class back into a DTO
5. Store the data back into the data access layer

### Application Layer

The application layer is the front of the web application. It receives input in the form of 
HTTP requests, calls to the service layer to act on that data, and responds with HTTP 
responses. In this project, "http requests/responses" would just be using all the views inside
`WebApplication/Views`. 

The requests and responses use seperate ViewModel classes under `WebApplication1/ViewModels`, 
and have extra C# attributes in each field that play nice with Razer page forms.

> [Model Validation Docs](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0)

These model classes aren't really object-oriented, as all properties can potentially be unset
or null (in the case of when a user leaves an input blank). However, this is fine as the 
*actual* validation is done inside the Domain layer. 

Obviously, this does mean repeating validation logic. But, because this is for the front-end,
having duplicates is good. This can, and will introduce bugs. However, if we want one single
source of truth with validation, C#'s model validation system cannot be used because of those
potentially nullable objects.

Shortcuts Taken:

 - The application layer will use the Domain layer's `Role` class, just to simplify things