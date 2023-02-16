using Api.Customers;
using Logic.Entities;
using Logic.Repositories;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class CustomerController : Controller
{
    private readonly MovieRepository _movieRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly CustomerService _customerService;
    
    
    public CustomerController(MovieRepository movieRepository, CustomerRepository customerRepository, CustomerService customerService)
    {
        _movieRepository = movieRepository;
        _customerRepository = customerRepository;
        _customerService = customerService;
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Get(long id)
    {
        var customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return NotFound();
        }

        var dto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            MoneySpent = customer.MoneySpent,
            Status = customer.Status.ToString(),
            StatusExpirationDate = customer.StatusExpirationDate,
            PurchasedMovies = customer.PurchasedMovies.Select(x => new PurchasedMovieDto
            {
                Price = x.Price,
                ExpirationDate = x.ExpirationDate,
                PurchaseDate = x.PurchaseDate,
                Movie = new MovieDto
                {
                    Id = x.Movie.Id,
                    Name = x.Movie.Name
                }
            }).ToList()
        };
        
        return Json(dto);
    }

    [HttpGet]
    public JsonResult GetList()
    {
        var customers = _customerRepository.GetList();
        var dtos = customers.Select(x => new CustomerInListDto
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email,
            MoneySpent = x.MoneySpent,
            Status = x.Status.ToString(),
            StatusExpirationDate = x.StatusExpirationDate
        }).ToList();
        
        return Json(dtos);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateCustomerDto item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_customerRepository.GetByEmail(item.Email) != null)
            {
                return BadRequest("Email already in use");
            }

            var customer = new Customer
            {
                Name = item.Name,
                Email = item.Email,
                MoneySpent = 0,
                Status = CustomerStatus.Regular
            };
            _customerRepository.Add(customer);
            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Name = item.Name;
            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost]
    [Route("{id}/movies")]
    public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
    {
        try
        {
            var movie = _movieRepository.GetById(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (customer.PurchasedMovies.Any(x =>
                    x.MovieId == movie.Id && (x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow)))
            {
                return BadRequest("You already bought that movie");
            }

            _customerService.PurchaseMovie(customer, movie);
            _customerRepository.SaveChanges();

            return Ok();

        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost]
    [Route("{id}/promotion")]
    public IActionResult PromoteCustomer(long id)
    {
        try
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (customer.Status == CustomerStatus.Advanced && (customer.StatusExpirationDate == null ||
                                                               customer.StatusExpirationDate < DateTime.UtcNow))
            {
                return BadRequest("Already advanced");
            }

            var success = _customerService.PromoteCustomer(customer);
            if (!success)
            {
                return BadRequest("get fucked");
            }

            _customerRepository.SaveChanges();
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}