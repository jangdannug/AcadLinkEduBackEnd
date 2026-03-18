using AcadLinkEduBackEnd.Application.Services;
using AcadLinkEduBackEnd.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Supabase.Postgrest.Exceptions;

namespace AcadLinkEduBackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notifService;

    public NotificationsController(NotificationService notifService)
    {
        _notifService = notifService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(int userId)
    {
        try
        {
            var notifications = await _notifService.GetNotificationsAsync(userId);
            var dtos = notifications.Select(u => new NotificationDto
            {
                Id = u.Id,
                UserId = u.UserId,
                Title = u.Title,
                Message = u.Message,
                Type = u.Type,
                IsRead = u.IsRead,
                CreatedAt = u.CreatedAt
            }).ToList();

            return Ok(dtos);
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

    // PUT /api/notifications/{id}/read
    // Body: { "isRead": true }
    // Toggles the read status for a single notification. Returns the updated item.
    //[HttpPut("{id}/read")]
    //public async Task<IActionResult> ToggleRead(string id, [FromBody] ToggleReadRequest body)
    //{
    //    await Task.Delay(300);

    //    lock (_lock)
    //    {
    //        var item = _store.FirstOrDefault(n => n.Id == id);
    //        if (item == null) return NotFound(new { success = false, message = "Notification not found" });

    //        item.IsRead = body.IsRead;
    //        return Ok(new { success = true, data = item });
    //    }
    //}

    //public class ToggleReadRequest
    //{
    //    public bool IsRead { get; set; }
    //}

    //// DELETE /api/notifications/{id}
    //// Deletes a notification and returns success flag.
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(string id)
    //{
    //    await Task.Delay(300);

    //    lock (_lock)
    //    {
    //        var index = _store.FindIndex(n => n.Id == id);
    //        if (index == -1) return NotFound(new { success = false, message = "Notification not found" });

    //        _store.RemoveAt(index);
    //        return Ok(new { success = true });
    //    }
    //}
}
