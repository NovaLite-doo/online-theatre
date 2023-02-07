using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logic.Entities;

public class Customer
{
    [Required]
    [MaxLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression(@"^(.+)@(.+)$", ErrorMessage = "Email is invalid")]
    public string Email { get; set; } = string.Empty;

    public CustomerStatus Status { get; set; }
    
    public DateTime? StatusExpirationDate { get; set; }
    
    public decimal MoneySpent { get; set; }
    public IList<PurchasedMovie> PurchasedMovies { get; set; } = new List<PurchasedMovie>();
    public long Id { get; set; }
}