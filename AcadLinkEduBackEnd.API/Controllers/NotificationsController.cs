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


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await _notifService.DeleteNotificationAsync(int.Parse(id));

        return Ok(new { success = true });
        
    }


    [HttpPatch("{id}/Read")]
    public async Task<IActionResult> ToggleRead(int id, [FromQuery] bool isRead)
    {
        await _notifService.ToggleNotificationReadAsync(id, isRead);
        return Ok(new { success = true });
    }

}
