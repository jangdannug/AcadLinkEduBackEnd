using AcadLinkEduBackEnd.Application.Services;
using AcadLinkEduBackEnd.Domain.DTO;
using AcadLinkEduBackEnd.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Supabase.Postgrest.Exceptions;
using System.Transactions;

namespace AcadLinkEduBackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;
    private readonly Supabase.Client _supabase;


    public SubmissionsController(SubmissionService submissionService, Supabase.Client supabase)
    {
        _submissionService = submissionService;
        _supabase = supabase;
    }

    // GET: api/Submissions?studentId=123
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? studentId)
    {
        try
        {
            var subs = await _submissionService.GetSubmissionsAsync(studentId);
            var data = subs.Select(s => new SubmissionsDto
            {
                Id = s.Id,
                ActivityId = s.ActivityId,
                StudentId = s.StudentId,
                FileUrl = s.FileUrl,
                FileName = s.FileName,
                SubmittedAt = s.SubmittedAt,
                Status = s.Status
            }).ToList();

            return Ok(data);
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

    // POST: api/Submissions
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromForm] CreateSubmissionDto request)
    {
        try
        {
            if (request.Files == null || request.Files.Count == 0)
                return BadRequest("No files uploaded");

            var fileUrls = new List<string>();

            foreach (var file in request.Files)
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                var fileBytes = ms.ToArray();

                await _supabase.Storage
                    .From("submissions")
                    .Upload(fileBytes, fileName);

                var publicUrl = _supabase.Storage
                    .From("submissions")
                    .GetPublicUrl(fileName);

                fileUrls.Add(publicUrl);
            }

            var created = await _submissionService.CreateSubmissionAsync(
                request.ActivityId!.Value,
                request.StudentId!.Value,
                string.Join(",", fileUrls),
                string.Join(",", request.Files.Select(f => f.FileName))
            );

            return Ok(created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
