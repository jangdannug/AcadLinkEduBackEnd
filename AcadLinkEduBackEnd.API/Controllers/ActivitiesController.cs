using AcadLinkEduBackEnd.Application.Services;
using AcadLinkEduBackEnd.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly ActivityService _activityService;
    private readonly NotificationService _notificationService;

    public ActivitiesController(ActivityService activityService, NotificationService notificationService)
    {
        _activityService = activityService;
        _notificationService = notificationService;
    }

    // GET: api/Activities?classId=xxx
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? classId)
    {
        var activities = await _activityService.GetActivitiesAsync(classId);
        var data = activities.Select(a => new ActivityDto
        {
            Id = a.Id,
            ClassId = a.ClassId,
            Title = a.Title,
            Description = a.Description,
            Deadline = a.Deadline,
            RequiredFiles = a.RequiredFiles
        }).ToList();
        return Ok(data);
    }

    // POST: api/Activities
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ActivityDto request)
    {
        try
        {
            var activity = await _activityService.CreateActivityAsync(request);

            var data = new ActivityDto
            {
                Id = activity.Id,
                ClassId = activity.ClassId,
                Title = activity.Title,
                Description = activity.Description,
                Deadline = activity.Deadline,
                RequiredFiles = activity.RequiredFiles

            };


            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // PUT: api/Activities/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActivityDto request)
    {
        try
        {
            var updated = await _activityService.UpdateActivityAsync(id, request);

            var data = new ActivityDto
            {
                Id = updated.Id,
                ClassId = updated.ClassId,
                Title = updated.Title,
                Description = updated.Description,
                Deadline = updated.Deadline,
                RequiredFiles = updated.RequiredFiles
            };
            return Ok(data);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // DELETE: api/Activities/{id}
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(string id)
    //{
    //    try
    //    {
    //        await _activityService.DeleteAsync(id);
    //        return Ok(new { success = true });
    //    }
    //    catch (KeyNotFoundException)
    //    {
    //        return NotFound();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ex.Message);
    //    }
    //}
}