using AcadLinkEduBackEnd.Application.Dtos;
using AcadLinkEduBackEnd.Application.Services;
using AcadLinkEduBackEnd.Domain.DTO;
using AcadLinkEduBackEnd.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Supabase.Postgrest.Exceptions;

namespace AcadLinkEduBackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : ControllerBase
{
    private readonly ClassService _classService;

    public ClassesController(ClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? studentId)
    {
        try
        {
            var classes = await _classService.GetAllAsync(studentId);
            return Ok(classes);
        }
        catch (PostgrestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] int? studentId)
    {
        try
        {
            var cls = await _classService.GetByIdAsync(id, studentId);
            if (cls == null) return NotFound();
            return Ok(cls);
        }
        catch (PostgrestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ClassRequest input)
    {
        try
        {
            var created = await _classService.CreateAsync(input);


            return Ok(new { success = true, id = created.Id });
        }
        catch (PostgrestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Class input)
    {
        try
        {
            var updated = await _classService.UpdateAsync(id, input);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (PostgrestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _classService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (PostgrestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
