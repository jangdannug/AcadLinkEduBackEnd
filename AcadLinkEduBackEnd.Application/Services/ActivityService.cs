using AcadLinkEduBackEnd.Domain.DTO;
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

    public async Task<Activity> CreateActivityAsync(ActivityDto activity)
    {
        // Insert activity
        var data = new Activity
        {
            ClassId = activity.ClassId,
            Title = activity.Title,
            Description = activity.Description,
            Deadline = activity.Deadline,
            RequiredFiles = activity.RequiredFiles
        };

        var insertResp = await _supabase.From<Activity>().Insert(data);
        var created = insertResp.Models.First();

        // Notify students enrolled in the class
        var enrollResp = await _supabase.From<Enrollment>().Where(e => e.ClassId == created.ClassId).Get();
        var enrollments = enrollResp.Models;

        foreach (var e in enrollments)
        {
            var notification = new Notification
            {
                UserId = (int)e.StudentId,
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

    public async Task<Activity> UpdateActivityAsync(int id, ActivityDto data)
    {
        var dataToUpdate = new Activity
        {
            ClassId = data.ClassId,
            Title = data.Title,
            Description = data.Description,
            Deadline = data.Deadline,
            RequiredFiles = data.RequiredFiles
        };
        var resp = await _supabase.From<Activity>().Where(a => a.Id == id).Update(dataToUpdate);
        var updated = resp.Models.FirstOrDefault();
        if (updated == null) throw new KeyNotFoundException("Activity not found");
        return updated;
    }

    //public async Task<Activity> DeleteActivityAsync(int id, ActivityDto data)
    //{
    //    var dataToUpdate = new Activity
    //    {
    //        ClassId = data.ClassId,
    //        Title = data.Title,
    //        Description = data.Description,
    //        Deadline = data.Deadline,
    //        RequiredFiles = data.RequiredFiles
    //    };
    //    var resp = await _supabase.From<Activity>().Where(a => a.Id == id).Update(dataToUpdate);
    //    var updated = resp.Models.FirstOrDefault();
    //    if (updated == null) throw new KeyNotFoundException("Activity not found");
    //    return updated;
    //}
}
