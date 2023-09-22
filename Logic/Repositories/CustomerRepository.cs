using Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logic.Repositories;

public class CustomerRepository : Repository<Customer>
{
    public CustomerRepository(OnlineTheatreDbContext context) : base(context)
    {
    }
    public Customer? GetByEmail(string email) => Context.Customers.SingleOrDefault(x => x.Email == email);
}