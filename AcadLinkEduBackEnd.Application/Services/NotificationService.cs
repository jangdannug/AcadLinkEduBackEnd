using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.Infrastructure;

namespace AcadLinkEduBackEnd.Application.Services;

public class NotificationService
{
    private readonly Supabase.Client _supabase;

    public NotificationService(SupabaseService supabaseService)
    {
        _supabase = supabaseService.Client;
    }

    public async Task<List<AcadLinkEduBackEnd.Domain.Entities.Notification>> GetNotificationsAsync(int userId)
    {
        var resp = await _supabase.From<AcadLinkEduBackEnd.Domain.Entities.Notification>().Where(n => n.UserId == userId).Get();

        return resp.Models.ToList();
    }

    public async Task<bool> ToggleNotificationReadAsync(int id, bool isRead)
    {
        var resp = await _supabase.From<AcadLinkEduBackEnd.Domain.Entities.Notification>().Where(n => n.Id == id)
            .Set(n => n.IsRead, isRead)
            .Update();
        return resp.Models != null && resp.Models.Any();
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        // Check existence
        var existing = await _supabase.From<AcadLinkEduBackEnd.Domain.Entities.Notification>().Where(n => n.Id == id).Get();
        if (!existing.Models.Any()) return false;

        // Delete
        await _supabase.From<AcadLinkEduBackEnd.Domain.Entities.Notification>().Where(n => n.Id == id).Delete();

        var check = await _supabase.From<AcadLinkEduBackEnd.Domain.Entities.Notification>().Where(n => n.Id == id).Get();
        return !check.Models.Any();
    }

    public async Task NotifyClassStudentsAsync(int? classId, string title, string message)
    {
        // Here you would query students enrolled in the class and send notifications
       var response = await _supabase
            .From<Enrollment>()
            .Select("student_id") // use DB column name
            .Where(n => n.ClassId == classId)
            .Get();

        var students = response.Models
            .Select(x => x.StudentId)
            .ToList();

        // For now, just log:
        foreach (var student in students) 
        {
            var notify = new AcadLinkEduBackEnd.Domain.Entities.Notification
            {
                UserId = (int)student,
                Title = title,
                Message = message,
                Type = "info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

             await _supabase
                .From<AcadLinkEduBackEnd.Domain.Entities.Notification>()
                .Insert(notify);

        }
    }

    public async Task NotifyUserCreateClassAsync(List<AcadLinkEduBackEnd.Domain.Entities.User> users, string title, string message)
    {
        foreach (var user in users)
        {
            var notify = new AcadLinkEduBackEnd.Domain.Entities.Notification
            {
                UserId = user.Id,
                Title = title,
                Message = message,
                Type = "info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase
                .From<AcadLinkEduBackEnd.Domain.Entities.Notification>()
                .Insert(notify);
        }
    }

    public async Task NotifyJoinClassAsync(string inviteCode, int? studentId)
    {
        var classInfo = await _supabase.From<Class>().Where(c => c.InviteCode == inviteCode).Get();
        var studentInfo = await _supabase.From<AcadLinkEduBackEnd.Domain.Entities.User>().Where(u => u.Id == studentId).Select("name").Single();

        var notify = new AcadLinkEduBackEnd.Domain.Entities.Notification
            {
                UserId = classInfo.Model.TeacherId,
                Title = $"{studentInfo.Name} Joined {classInfo.Model.Name} class",
                Message = $"{studentInfo.Name} joined {classInfo.Model.Name} description: {classInfo.Model.Description}",
                Type = "info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase
                .From<AcadLinkEduBackEnd.Domain.Entities.Notification>()
                .Insert(notify);
        
    }
}
