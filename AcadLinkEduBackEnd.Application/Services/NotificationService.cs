using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.Infrastructure;
using Supabase;
using Supabase.Postgrest;

namespace AcadLinkEduBackEnd.Application.Services;

public class NotificationService
{
    private readonly Supabase.Client _supabase;

    public NotificationService(SupabaseService supabaseService)
    {
        _supabase = supabaseService.Client;
    }

    public async Task<List<Notification>> GetNotificationsAsync(int userId)
    {
        var resp = await _supabase.From<Notification>().Where(n => n.UserId == userId).Get();
        return resp.Models.ToList();
    }

    public async Task<bool> ToggleNotificationReadAsync(int id, bool isRead)
    {
        var resp = await _supabase.From<Notification>().Where(n => n.Id == id).Update(new Notification { IsRead = isRead });
        return resp.Models != null && resp.Models.Any();
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        // Check existence
        var existing = await _supabase.From<Notification>().Where(n => n.Id == id).Get();
        if (!existing.Models.Any()) return false;

        // Delete
        await _supabase.From<Notification>().Where(n => n.Id == id).Delete();

        var check = await _supabase.From<Notification>().Where(n => n.Id == id).Get();
        return !check.Models.Any();
    }
}
