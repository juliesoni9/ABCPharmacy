using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Models;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Medicine>>> GetAll([FromQuery] string? search)
    {
        var medicines = await _medicineService.GetAllAsync(search);
        return Ok(medicines);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Medicine>> GetById(Guid id)
    {
        var medicine = await _medicineService.GetByIdAsync(id);
        if (medicine is null)
        {
            return NotFound();
        }

        return Ok(medicine);
    }

    [HttpPost]
    public async Task<ActionResult<Medicine>> Create([FromBody] CreateMedicineRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (medicine, error) = await _medicineService.CreateAsync(request);
        if (medicine is null)
        {
            return BadRequest(new { message = error });
        }

        return CreatedAtAction(nameof(GetById), new { id = medicine.Id }, medicine);
    }
}
