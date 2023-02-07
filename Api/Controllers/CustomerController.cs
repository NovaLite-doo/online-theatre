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

        return Ok(customer);
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var customers = _customerRepository.GetList();
        return Ok(customers);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Customer item)
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

            item.Id = 0;
            item.Status = CustomerStatus.Regular;
            _customerRepository.Add(item);
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
    public IActionResult Update(long id, [FromBody] Customer item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _customerRepository.GetById(item.Id);
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