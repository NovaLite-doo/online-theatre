using Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logic.Repositories;

public class CustomerRepository : Repository<Customer>
{
    public CustomerRepository(OnlineTheatreDbContext context) : base(context)
    {
    }

    public IReadOnlyList<Customer> GetList() => 
        _context.Customers.Include(x => x.PurchasedMovies).ToList();

    public Customer? GetByEmail(string email) => _context.Customers.SingleOrDefault(x => x.Email == email);
}