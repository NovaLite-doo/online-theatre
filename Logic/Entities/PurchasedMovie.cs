namespace Logic.Entities;

public class PurchasedMovie
{
    public long Id { get; set; }
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long CustomerId { get; set; }
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}