using Logic.Entities;

namespace Logic.Services;

public class CustomerService
{
    private readonly MovieService _movieService;

    public CustomerService(MovieService movieService)
    {
        _movieService = movieService;
    }

    public void PurchaseMovie(Customer customer, Movie movie)
    {
        var expirationDate = _movieService.GetExpirationDate(movie.LicensingModel);
        var price = CalculatePrice(customer.Status, customer.StatusExpirationDate, movie.LicensingModel);

        var purchasedMovie = new PurchasedMovie
        {
            MovieId = movie.Id,
            CustomerId = customer.Id,
            ExpirationDate = expirationDate,
            Price = price
        };
        
        customer.PurchasedMovies.Add(purchasedMovie);
        customer.MoneySpent += price;
    }

    public bool PromoteCustomer(Customer customer)
    {
        //2+ active movies last 30days
        if (customer.PurchasedMovies.Count(x =>
                x.ExpirationDate == null || x.ExpirationDate >= DateTime.UtcNow.AddDays(-30)) < 2)
        {
            return false;
        }
        
        // at least $100 spent last year
        if (customer.PurchasedMovies.Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1)).Sum(x => x.Price) < 100m)
        {
            return false;
        }

        customer.Status = CustomerStatus.Advanced;
        customer.StatusExpirationDate = DateTime.UtcNow.AddYears(1);
        return true;
    }

    private decimal CalculatePrice(CustomerStatus status, DateTime? statusExpirationDate, LicensingModel licensingModel)
    {
        decimal price = licensingModel switch
        {
            LicensingModel.TwoDays => 4,
            LicensingModel.LifeLong => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(licensingModel), licensingModel, null)
        };

        if (status == CustomerStatus.Advanced &&
            (statusExpirationDate == null || statusExpirationDate.Value >= DateTime.UtcNow))
        {
            price *= 0.75m;
        }

        return price;
    }
}