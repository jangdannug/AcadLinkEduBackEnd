using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.Infrastructure;
using Supabase;
using Supabase.Postgrest;

namespace AcadLinkEduBackEnd.Application.Services;

public class ActivityService
{
    private readonly Supabase.Client _supabase;

    public ActivityService(SupabaseService supabaseService)
    {
        _supabase = supabaseService.Client;
    }

    public async Task<List<Activity>> GetActivitiesAsync(int? classId = null)
    {
        if (classId.HasValue)
        {
            var resp = await _supabase.From<Activity>().Where(a => a.ClassId == classId.Value).Get();
            return resp.Models.ToList();
        }

        var response = await _supabase.From<Activity>().Get();
        return response.Models.ToList();
    }

    public async Task<Activity> CreateActivityAsync(Activity activity)
    {
        // Insert activity
        var insertResp = await _supabase.From<Activity>().Insert(activity);
        var created = insertResp.Models.First();

        // Notify students enrolled in the class
        var enrollResp = await _supabase.From<Enrollment>().Where(e => e.ClassId == created.ClassId).Get();
        var enrollments = enrollResp.Models;

        foreach (var e in enrollments)
        {
            var notification = new Notification
            {
                UserId = e.StudentId,
                Title = "New Mission Deployed",
                Message = $"New activity: {created.Title} in your class!",
                Type = "task",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.From<Notification>().Insert(notification);
        }

        return created;
    }

    public async Task<Activity> UpdateActivityAsync(int id, Activity data)
    {
        var resp = await _supabase.From<Activity>().Where(a => a.Id == id).Update(data);
        var updated = resp.Models.FirstOrDefault();
        if (updated == null) throw new KeyNotFoundException("Activity not found");
        return updated;
    }
}
