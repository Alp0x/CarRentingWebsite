using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly CarContext _context;

    public CarController(CarContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars(string Genre)
    {
        var cars = _context.Cars.Select(x => x);
        if (!string.IsNullOrEmpty(Genre))
        {
            cars = cars.Where(x => x.Brand.StartsWith(Genre));
        }
        return await cars.OrderByDescending(x => x.Status).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCar(int id)
    {
        return await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);
    }

    [HttpPost]
    public async Task<IActionResult> PostCar(Car car)
    {
        if (!ModelState.IsValid) return BadRequest();

        await _context.AddAsync(car);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id)
    {
        var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);

        car.Status = !car.Status;
        await _context.SaveChangesAsync();
        return Ok();
    }
}
